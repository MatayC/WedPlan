-- =============================================================================
-- WedPlan – Vollständiges Datenbank-Schema für Supabase
-- =============================================================================
-- Mehrbenutzer-Hochzeitsplanung mit geteilten Gruppen.
--
-- ANLEITUNG:
--   1. Supabase-Dashboard öffnen -> linkes Menü -> "SQL Editor"
--   2. "New query" klicken
--   3. Den GESAMTEN Inhalt dieser Datei hineinkopieren
--   4. Auf "Run" klicken
--
-- Das Skript ist "idempotent" – du kannst es gefahrlos erneut ausführen.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 0) Aufräumen alter Objekte (falls schon vorher etwas angelegt wurde)
--    Auskommentiert lassen, außer du willst wirklich ALLES neu aufsetzen.
-- -----------------------------------------------------------------------------
-- DROP TABLE IF EXISTS wedding_settings CASCADE;
-- DROP TABLE IF EXISTS seating_tables   CASCADE;
-- DROP TABLE IF EXISTS schedule_items   CASCADE;
-- DROP TABLE IF EXISTS tasks            CASCADE;
-- DROP TABLE IF EXISTS apartment_items  CASCADE;
-- DROP TABLE IF EXISTS budget_items     CASCADE;
-- DROP TABLE IF EXISTS guests           CASCADE;
-- DROP TABLE IF EXISTS wedding_members  CASCADE;
-- DROP TABLE IF EXISTS weddings         CASCADE;
-- DROP TABLE IF EXISTS profiles         CASCADE;


-- -----------------------------------------------------------------------------
-- 1) PROFILES – ein Profil pro registriertem Benutzer (mit Nutzername)
-- -----------------------------------------------------------------------------
create table if not exists profiles (
	id          uuid        primary key references auth.users (id) on delete cascade,
	username    text        not null,
	email       text,
	created_at  timestamptz not null default now()
);

alter table profiles enable row level security;


-- -----------------------------------------------------------------------------
-- 2) WEDDINGS – eine Zeile pro Hochzeit/Gruppe, mit Einladungs-Code
-- -----------------------------------------------------------------------------
create table if not exists weddings (
	id           uuid        primary key default gen_random_uuid(),
	name         text        not null default 'Unsere Hochzeit',
	invite_code  text        not null unique,
	created_by   uuid        references auth.users (id) on delete set null,
	created_at   timestamptz not null default now()
);

alter table weddings enable row level security;

-- Sicherheitsnetz: Spalten nachrüsten falls Tabelle aus alter Version stammt.
alter table weddings add column if not exists created_by uuid references auth.users (id) on delete set null;
alter table weddings add column if not exists invite_code text unique;
update weddings set invite_code = upper(substr(replace(gen_random_uuid()::text, '-', ''), 1, 6)) where invite_code is null;
alter table weddings alter column invite_code set not null;


-- -----------------------------------------------------------------------------
-- 3) WEDDING_MEMBERS – verknüpft Benutzer mit Hochzeiten (n:m)
-- -----------------------------------------------------------------------------
create table if not exists wedding_members (
	wedding_id  uuid        not null references weddings (id) on delete cascade,
	user_id     uuid        not null references auth.users (id) on delete cascade,
	joined_at   timestamptz not null default now(),
	primary key (wedding_id, user_id)
);

alter table wedding_members enable row level security;

-- Rollen & seitenbasierte Berechtigungen nachrüsten (Sicherheitsnetz bei alten Tabellen).
-- role: 'admin' (voller Zugriff + Rechteverwaltung) oder 'member' (Rechte je Seite).
alter table wedding_members add column if not exists role text not null default 'member';
alter table wedding_members add column if not exists can_edit_guests    boolean not null default false;
alter table wedding_members add column if not exists can_edit_budget    boolean not null default false;
alter table wedding_members add column if not exists can_edit_tasks     boolean not null default false;
alter table wedding_members add column if not exists can_edit_schedule  boolean not null default false;
alter table wedding_members add column if not exists can_edit_seating   boolean not null default false;
alter table wedding_members add column if not exists can_edit_settings  boolean not null default false;
alter table wedding_members add column if not exists can_edit_apartment boolean not null default false;
alter table wedding_members add column if not exists can_delete_project boolean not null default false;


-- -----------------------------------------------------------------------------
-- 4) Hilfsfunktion: Ist der aktuelle Benutzer Mitglied dieser Hochzeit?
--    SECURITY DEFINER umgeht RLS innerhalb der Funktion und verhindert
--    unendliche Rekursion in den Policies.
-- -----------------------------------------------------------------------------
drop function if exists is_wedding_member(uuid) cascade;
create or replace function is_wedding_member(target_wedding uuid)
returns boolean
language sql
security definer
set search_path = public
as $$
	select exists (
		select 1
		from wedding_members
		where wedding_id = target_wedding
		  and user_id = auth.uid()
	);
$$;

-- Ist der aktuelle Benutzer Admin dieser Hochzeit?
drop function if exists is_wedding_admin(uuid) cascade;
create or replace function is_wedding_admin(target_wedding uuid)
returns boolean
language sql
security definer
set search_path = public
as $$
	select exists (
		select 1
		from wedding_members
		where wedding_id = target_wedding
		  and user_id = auth.uid()
		  and role = 'admin'
	);
$$;

-- Darf der aktuelle Benutzer die angegebene Seite bearbeiten?
-- Admins dürfen immer; Mitglieder nur bei erteiltem Recht je Seite.
drop function if exists can_edit_page(uuid, text) cascade;
create or replace function can_edit_page(target_wedding uuid, page text)
returns boolean
language sql
security definer
set search_path = public
as $$
	select exists (
		select 1
		from wedding_members m
		where m.wedding_id = target_wedding
		  and m.user_id = auth.uid()
		  and (
			m.role = 'admin'
			or (page = 'guests'   and m.can_edit_guests)
			or (page = 'budget'   and m.can_edit_budget)
			or (page = 'tasks'    and m.can_edit_tasks)
			or (page = 'schedule' and m.can_edit_schedule)
			or (page = 'seating'  and m.can_edit_seating)
			or (page = 'settings' and m.can_edit_settings)
			or (page = 'apartment' and m.can_edit_apartment)
		  )
	);
$$;


-- -----------------------------------------------------------------------------
-- 5) DATENTABELLEN – jeweils an eine Hochzeit gebunden (wedding_id)
-- -----------------------------------------------------------------------------

-- Gäste
create table if not exists guests (
	id                uuid        primary key default gen_random_uuid(),
	wedding_id        uuid        not null references weddings (id) on delete cascade,
	first_name        text        not null default '',
	last_name         text        not null default '',
	email             text,
	phone             text,
	category          int         not null default 0,
	group_name        text        not null default 'Sonstige',
	rsvp              int         not null default 0,
	plus_ones         int         not null default 0,
	table_id          uuid,
	menu              int         not null default 0,
	allergies         text,
	attends_reception boolean     not null default true,
	notes             text,
	created_at        timestamptz not null default now()
);

-- Budget-Posten
create table if not exists budget_items (
	id             uuid        primary key default gen_random_uuid(),
	wedding_id     uuid        not null references weddings (id) on delete cascade,
	title          text        not null default '',
	category       text        not null default '',
	estimated_cost numeric     not null default 0,
	actual_cost    numeric     not null default 0,
	paid_amount    numeric     not null default 0,
	status         int         not null default 0,
	vendor         text,
	due_date       timestamptz,
	notes          text,
	created_at     timestamptz not null default now()
);

-- Wohnungsplanung – individuelle Kostenposten
create table if not exists apartment_items (
	id             uuid        primary key default gen_random_uuid(),
	wedding_id     uuid        not null references weddings (id) on delete cascade,
	title          text        not null default '',
	category       text        not null default '',
	estimated_cost numeric     not null default 0,
	actual_cost    numeric     not null default 0,
	paid_amount    numeric     not null default 0,
	status         int         not null default 0,
	vendor         text,
	due_date       timestamptz,
	notes          text,
	created_at     timestamptz not null default now()
);

-- Aufgaben / Checkliste
create table if not exists tasks (
	id           uuid        primary key default gen_random_uuid(),
	wedding_id   uuid        not null references weddings (id) on delete cascade,
	title        text        not null default '',
	description  text,
	due_date     timestamptz,
	status       int         not null default 0,
	priority     int         not null default 1,
	phase        int         not null default 0,
	responsible  text,
	category     text,
	created_at   timestamptz not null default now()
);

-- Zeitplan / Meilensteine
create table if not exists schedule_items (
	id                 uuid        primary key default gen_random_uuid(),
	wedding_id         uuid        not null references weddings (id) on delete cascade,
	title              text        not null default '',
	description        text,
	start_time         timestamptz,
	end_time           timestamptz,
	location           text,
	responsible_person text,
	category           int         not null default 0,
	is_milestone       boolean     not null default false,
	created_at         timestamptz not null default now()
);

-- Sitzordnung – Tische
create table if not exists seating_tables (
	id                 uuid        primary key default gen_random_uuid(),
	wedding_id         uuid        not null references weddings (id) on delete cascade,
	name               text        not null default '',
	shape              int         not null default 0,
	capacity           int         not null default 8,
	assigned_guest_ids uuid[]      not null default '{}',
	created_at         timestamptz not null default now()
);

-- Einstellungen – genau eine Zeile pro Hochzeit
create table if not exists wedding_settings (
	wedding_id    uuid        primary key references weddings (id) on delete cascade,
	partner1_name text        not null default '',
	partner2_name text        not null default '',
	wedding_date  timestamptz,
	location      text,
	total_budget  numeric     not null default 0,
	currency      text        not null default 'EUR',
	is_dark_mode  boolean     not null default false,
	cover_image_url text,
	gallery_image1_url text,
	gallery_image2_url text,
	savings_partner1 numeric not null default 0,
	savings_partner2 numeric not null default 0,
	monthly_saving_partner1 numeric not null default 0,
	monthly_saving_partner2 numeric not null default 0,
	updated_at    timestamptz not null default now()
);

-- Sicherheitsnetz: Titelbild-Spalte nachrüsten, falls Tabelle aus alter Version stammt.
alter table wedding_settings add column if not exists cover_image_url text;

-- Sicherheitsnetz: dezente Dashboard-Bilder (Impressionen) nachrüsten.
alter table wedding_settings add column if not exists gallery_image1_url text;
alter table wedding_settings add column if not exists gallery_image2_url text;

-- Sicherheitsnetz: Sparplan-Spalten nachrüsten.
alter table wedding_settings add column if not exists savings_partner1 numeric not null default 0;
alter table wedding_settings add column if not exists savings_partner2 numeric not null default 0;
alter table wedding_settings add column if not exists monthly_saving_partner1 numeric not null default 0;
alter table wedding_settings add column if not exists monthly_saving_partner2 numeric not null default 0;

-- RLS auf allen Datentabellen aktivieren
alter table guests           enable row level security;
alter table budget_items     enable row level security;
alter table apartment_items  enable row level security;
alter table tasks            enable row level security;
alter table schedule_items   enable row level security;
alter table seating_tables   enable row level security;
alter table wedding_settings enable row level security;


-- -----------------------------------------------------------------------------
-- 6) RLS-POLICIES
-- -----------------------------------------------------------------------------

-- PROFILES: jeder darf sein eigenes Profil lesen/anlegen/ändern.
-- Zusätzlich: Profile von Mitgliedern derselben Hochzeit lesbar (für Mitgliederliste).
drop policy if exists "profiles_select_own_or_group" on profiles;
create policy "profiles_select_own_or_group" on profiles
	for select using (
		id = auth.uid()
		or exists (
			select 1
			from wedding_members m1
			join wedding_members m2 on m1.wedding_id = m2.wedding_id
			where m1.user_id = auth.uid()
			  and m2.user_id = profiles.id
		)
	);

drop policy if exists "profiles_insert_own" on profiles;
create policy "profiles_insert_own" on profiles
	for insert with check (id = auth.uid());

drop policy if exists "profiles_update_own" on profiles;
create policy "profiles_update_own" on profiles
	for update using (id = auth.uid());


-- WEDDINGS:
--   SELECT: nur Mitglieder der Hochzeit ODER anhand des invite_code (zum Beitreten).
--   INSERT: jeder eingeloggte Benutzer darf eine Hochzeit anlegen (created_by = self).
--   UPDATE: nur Mitglieder.
drop policy if exists "weddings_select_members" on weddings;
create policy "weddings_select_members" on weddings
	for select using (is_wedding_member(id));

drop policy if exists "weddings_insert_self" on weddings;
create policy "weddings_insert_self" on weddings
	for insert with check (created_by = auth.uid());

drop policy if exists "weddings_update_members" on weddings;
create policy "weddings_update_members" on weddings
	for update using (is_wedding_member(id));


-- WEDDING_MEMBERS:
--   SELECT: man sieht Mitgliedschaften der eigenen Hochzeiten.
--   INSERT: man darf sich selbst als Mitglied eintragen (Beitritt/Anlegen).
--   DELETE: man darf sich selbst entfernen (Austritt).
drop policy if exists "members_select_group" on wedding_members;
create policy "members_select_group" on wedding_members
	for select using (
		user_id = auth.uid()
		or is_wedding_member(wedding_id)
	);

drop policy if exists "members_insert_self" on wedding_members;
create policy "members_insert_self" on wedding_members
	for insert with check (user_id = auth.uid());

drop policy if exists "members_delete_self" on wedding_members;
create policy "members_delete_self" on wedding_members
	for delete using (user_id = auth.uid());


-- DATENTABELLEN: Lesen (SELECT) für alle Mitglieder, Schreiben (INSERT/UPDATE/DELETE)
-- nur für Mitglieder mit Bearbeitungsrecht der jeweiligen Seite (oder Admins).

-- guests
drop policy if exists "guests_all_members" on guests;
drop policy if exists "guests_select_members" on guests;
create policy "guests_select_members" on guests
	for select using (is_wedding_member(wedding_id));
drop policy if exists "guests_write_editors" on guests;
create policy "guests_write_editors" on guests
	for all using (can_edit_page(wedding_id, 'guests'))
	with check (can_edit_page(wedding_id, 'guests'));

-- budget_items
drop policy if exists "budget_all_members" on budget_items;
drop policy if exists "budget_select_members" on budget_items;
create policy "budget_select_members" on budget_items
	for select using (is_wedding_member(wedding_id));
drop policy if exists "budget_write_editors" on budget_items;
create policy "budget_write_editors" on budget_items
	for all using (can_edit_page(wedding_id, 'budget'))
	with check (can_edit_page(wedding_id, 'budget'));

-- apartment_items
drop policy if exists "apartment_select_members" on apartment_items;
create policy "apartment_select_members" on apartment_items
	for select using (is_wedding_member(wedding_id));
drop policy if exists "apartment_write_editors" on apartment_items;
create policy "apartment_write_editors" on apartment_items
	for all using (can_edit_page(wedding_id, 'apartment'))
	with check (can_edit_page(wedding_id, 'apartment'));

-- tasks
drop policy if exists "tasks_all_members" on tasks;
drop policy if exists "tasks_select_members" on tasks;
create policy "tasks_select_members" on tasks
	for select using (is_wedding_member(wedding_id));
drop policy if exists "tasks_write_editors" on tasks;
create policy "tasks_write_editors" on tasks
	for all using (can_edit_page(wedding_id, 'tasks'))
	with check (can_edit_page(wedding_id, 'tasks'));

-- schedule_items
drop policy if exists "schedule_all_members" on schedule_items;
drop policy if exists "schedule_select_members" on schedule_items;
create policy "schedule_select_members" on schedule_items
	for select using (is_wedding_member(wedding_id));
drop policy if exists "schedule_write_editors" on schedule_items;
create policy "schedule_write_editors" on schedule_items
	for all using (can_edit_page(wedding_id, 'schedule'))
	with check (can_edit_page(wedding_id, 'schedule'));

-- seating_tables
drop policy if exists "seating_all_members" on seating_tables;
drop policy if exists "seating_select_members" on seating_tables;
create policy "seating_select_members" on seating_tables
	for select using (is_wedding_member(wedding_id));
drop policy if exists "seating_write_editors" on seating_tables;
create policy "seating_write_editors" on seating_tables
	for all using (can_edit_page(wedding_id, 'seating'))
	with check (can_edit_page(wedding_id, 'seating'));

-- wedding_settings
drop policy if exists "settings_all_members" on wedding_settings;
drop policy if exists "settings_select_members" on wedding_settings;
create policy "settings_select_members" on wedding_settings
	for select using (is_wedding_member(wedding_id));
drop policy if exists "settings_write_editors" on wedding_settings;
create policy "settings_write_editors" on wedding_settings
	for all using (can_edit_page(wedding_id, 'settings'))
	with check (can_edit_page(wedding_id, 'settings'));


-- -----------------------------------------------------------------------------
-- 7) TRIGGER: Beim Registrieren automatisch ein Profil anlegen
--    Nutzername kommt aus den user_metadata (wird beim SignUp mitgegeben).
-- -----------------------------------------------------------------------------
create or replace function handle_new_user()
returns trigger
language plpgsql
security definer
set search_path = public
as $$
begin
	insert into public.profiles (id, username, email)
	values (
		new.id,
		coalesce(new.raw_user_meta_data ->> 'username', split_part(new.email, '@', 1)),
		new.email
	)
	on conflict (id) do nothing;
	return new;
end;
$$;

drop trigger if exists on_auth_user_created on auth.users;
create trigger on_auth_user_created
	after insert on auth.users
	for each row execute function handle_new_user();


-- -----------------------------------------------------------------------------
-- 8) FUNKTION: Einer Hochzeit per Einladungs-Code beitreten
--    Gibt die wedding_id zurück oder NULL, wenn der Code ungültig ist.
-- -----------------------------------------------------------------------------
create or replace function join_wedding_by_code(code text)
returns uuid
language plpgsql
security definer
set search_path = public
as $$
declare
	target uuid;
begin
	select id into target from weddings where invite_code = upper(trim(code));

	if target is null then
		return null;
	end if;

	insert into wedding_members (wedding_id, user_id)
	values (target, auth.uid())
	on conflict do nothing;

	return target;
end;
$$;


-- -----------------------------------------------------------------------------
-- 9) FUNKTION: Neue Hochzeit anlegen + Ersteller als Mitglied eintragen
--    Erzeugt automatisch einen kurzen, gut lesbaren Einladungs-Code.
-- -----------------------------------------------------------------------------
create or replace function create_wedding(wedding_name text)
returns uuid
language plpgsql
security definer
set search_path = public
as $$
declare
	new_id   uuid;
	new_code text;
begin
	-- 6-stelligen Code aus einer UUID ableiten (immer verfügbar, keine Extension nötig)
	loop
		new_code := upper(substr(replace(gen_random_uuid()::text, '-', ''), 1, 6));
		exit when not exists (select 1 from weddings where invite_code = new_code);
	end loop;

	insert into weddings (name, invite_code, created_by)
	values (coalesce(nullif(trim(wedding_name), ''), 'Unsere Hochzeit'), new_code, auth.uid())
	returning id into new_id;

	-- Ersteller wird Admin mit vollen Bearbeitungsrechten.
	insert into wedding_members (
		wedding_id, user_id, role,
		can_edit_guests, can_edit_budget, can_edit_tasks,
		can_edit_schedule, can_edit_seating, can_edit_settings, can_edit_apartment, can_delete_project
	)
	values (new_id, auth.uid(), 'admin', true, true, true, true, true, true, true, true);

	return new_id;
end;
$$;

-- -----------------------------------------------------------------------------
-- 10) FUNKTION: Eine Hochzeit löschen (nur für Mitglieder).
--     Löscht dank ON DELETE CASCADE automatisch alle zugehörigen Daten
--     (Gäste, Budget, Aufgaben, Zeitplan, Tische, Einstellungen, Mitglieder).
-- -----------------------------------------------------------------------------
create or replace function delete_wedding(target_wedding uuid)
returns boolean
language plpgsql
security definer
set search_path = public
as $$
begin
	-- Nur Admins oder Mitglieder mit ausdrücklichem Löschrecht dürfen löschen.
	if not exists (
		select 1 from wedding_members
		where wedding_id = target_wedding
		  and user_id = auth.uid()
		  and (role = 'admin' or can_delete_project)
	) then
		return false;
	end if;

	delete from weddings where id = target_wedding;
	return true;
end;
$$;

-- -----------------------------------------------------------------------------
-- 11) FUNKTION: Berechtigungen eines Mitglieds setzen (nur Admins).
--     Setzt Rolle und Bearbeitungsrechte je Seite in einem Aufruf.
--     Gibt true zurück, wenn der Aufrufer Admin ist und die Änderung erfolgte.
-- -----------------------------------------------------------------------------
-- Alte Signatur (ohne edit_apartment) entfernen, falls vorhanden.
drop function if exists update_member_permissions(
	uuid, uuid, text, boolean, boolean, boolean, boolean, boolean, boolean, boolean
) cascade;

create or replace function update_member_permissions(
	target_wedding uuid,
	target_user uuid,
	new_role text,
	edit_guests boolean,
	edit_budget boolean,
	edit_tasks boolean,
	edit_schedule boolean,
	edit_seating boolean,
	edit_settings boolean,
	edit_apartment boolean,
	delete_project boolean
)
returns boolean
language plpgsql
security definer
set search_path = public
as $$
begin
	-- Nur Admins der Hochzeit dürfen Rechte vergeben.
	if not is_wedding_admin(target_wedding) then
		return false;
	end if;

	update wedding_members
	set role               = case when new_role = 'admin' then 'admin' else 'member' end,
		can_edit_guests    = coalesce(edit_guests, false),
		can_edit_budget    = coalesce(edit_budget, false),
		can_edit_tasks     = coalesce(edit_tasks, false),
		can_edit_schedule  = coalesce(edit_schedule, false),
		can_edit_seating   = coalesce(edit_seating, false),
		can_edit_settings  = coalesce(edit_settings, false),
		can_edit_apartment = coalesce(edit_apartment, false),
		can_delete_project = coalesce(delete_project, false)
	where wedding_id = target_wedding
	  and user_id = target_user;

	return found;
end;
$$;

-- -----------------------------------------------------------------------------
-- 12) STORAGE: Öffentlicher Bucket für Titelbilder ("wedding-covers").
--     Bilder sind öffentlich lesbar; Schreiben/Löschen nur für angemeldete
--     Benutzer, die Mitglied der jeweiligen Hochzeit sind (Ordnername = wedding_id).
-- -----------------------------------------------------------------------------
insert into storage.buckets (id, name, public)
values ('wedding-covers', 'wedding-covers', true)
on conflict (id) do update set public = true;

-- Jeder darf Titelbilder lesen (Bucket ist öffentlich).
drop policy if exists "covers_public_read" on storage.objects;
create policy "covers_public_read" on storage.objects
	for select using (bucket_id = 'wedding-covers');

-- Hochladen nur für Mitglieder der Hochzeit (erster Ordner = wedding_id).
drop policy if exists "covers_member_insert" on storage.objects;
create policy "covers_member_insert" on storage.objects
	for insert with check (
		bucket_id = 'wedding-covers'
		and is_wedding_member((split_part(name, '/', 1))::uuid)
	);

-- Aktualisieren nur für Mitglieder der Hochzeit.
drop policy if exists "covers_member_update" on storage.objects;
create policy "covers_member_update" on storage.objects
	for update using (
		bucket_id = 'wedding-covers'
		and is_wedding_member((split_part(name, '/', 1))::uuid)
	);

-- Löschen nur für Mitglieder der Hochzeit.
drop policy if exists "covers_member_delete" on storage.objects;
create policy "covers_member_delete" on storage.objects
	for delete using (
		bucket_id = 'wedding-covers'
		and is_wedding_member((split_part(name, '/', 1))::uuid)
	);

-- -----------------------------------------------------------------------------
-- 13) REALTIME: Änderungen an wedding_settings live an Clients senden.
-- -----------------------------------------------------------------------------
alter publication supabase_realtime add table wedding_settings;

-- =============================================================================
-- FERTIG. Wenn "Success. No rows returned" erscheint, hat alles geklappt.
-- =============================================================================
