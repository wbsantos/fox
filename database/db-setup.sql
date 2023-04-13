--create database fox
/* 
// to recreate all tables, they can be dropped using the commands bellow
drop table document_permission;
drop table document_metadata;
drop table document;
drop table system_permission;
drop table user_group;
drop table stamp;
drop table user_account;
drop table group_account;
drop table holder;
*/

create table if not exists holder --it's either an user or a group id
(
	id uuid default gen_random_uuid() not null,

	constraint pk_holder primary key(id)
);

create table if not exists group_account
(
	id uuid default gen_random_uuid() not null,
	name varchar(255) not null,

	constraint pk_group_account primary key(id),
	constraint fk_group_account_to_holder foreign key (id) references holder(id)
);

create table if not exists user_account
(
	id uuid default gen_random_uuid() not null,
	email varchar(255) not null,
	login varchar(63) not null,
	password bytea not null,
	salt bytea not null,
	hash_method int not null,
	name varchar(255) not null,

	constraint pk_user_account primary key(id),
	constraint fk_user_account_to_holder foreign key (id) references holder(id)
);

create table if not exists stamp
(
	id int generated always as identity not null,
	user_id uuid,
	system_version varchar(255),
	created_at timestamptz,

	constraint pk_stamp primary key(id),
	constraint fk_stamp_user_account foreign key (user_id) references user_account(id)
);

create table if not exists user_group
(
	id int generated always as identity not null,
	user_id uuid not null,
	group_id uuid not null,
	stamp_id int not null,

	constraint pk_user_group primary key(id),
	constraint fk_user_group_to_user_account foreign key (user_id) references user_account(id),
	constraint fk_user_group_to_group_account foreign key (group_id) references group_account(id),
	constraint fk_user_group_to_stamp foreign key (stamp_id) references stamp(id)	
);

create table if not exists system_permission
(
	id int generated always as identity not null,
	stamp_id int not null,
	holder_id uuid not null,
	permission varchar(255) not null,

	constraint pk_system_permission primary key(id),
	constraint fk_system_permission_to_holder foreign key (holder_id) references holder(id),
	constraint fk_system_permission_to_stamp foreign key (stamp_id) references stamp(id)	
);

create table if not exists document
(
	id uuid default gen_random_uuid() not null,
	stamp_id int not null,
	name varchar(255),
	file_size_bytes bigint,

	constraint pk_document primary key(id),
	constraint fk_document_to_stamp foreign key (stamp_id) references stamp(id)	
);

create table if not exists document_metadata
(
	id int generated always as identity not null,
	document_id uuid not null,
	key varchar(255) not null,
	value varchar(1023),

	constraint pk_document_metadata primary key(id),
	constraint fk_document_metadata_to_document foreign key (document_id) references document(id)
);

do $$
begin
if not exists (select 1 from pg_type where typname = 'metadata_dictionary') then
	create type metadata_dictionary as
	(
		key varchar(255),
		value varchar(1023)
	);
    end if;
end$$;


create table if not exists document_permission
(
	id int generated always as identity not null,
	stamp_id int not null,
	holder_id uuid not null, 
	document_id uuid not null,
	permission varchar(255) not null,

	constraint pk_document_permission primary key(id),
	constraint fk_document_permission_to_holder foreign key (holder_id) references holder(id),
	constraint fk_document_permission_to_document foreign key (document_id) references document(id),
	constraint fk_document_permission_to_stamp foreign key (stamp_id) references stamp(id)
);

--creating indexes
create unique index if not exists un_user_account_01 on user_account(login);
create unique index if not exists un_group_account_01 on group_account(name);
create unique index if not exists un_user_group_01 on user_group(group_id, user_id);
create index if not exists in_stamp_01 on stamp(user_id);
create index if not exists in_stamp_02 on stamp(created_at);
create index if not exists in_user_group_01 on user_group(user_id);
create index if not exists in_user_group_02 on user_group(group_id);
create index if not exists in_system_permission_01 on system_permission(holder_id);
create index if not exists in_document_01 on document(name);
create index if not exists in_document_metadata_01 on document_metadata(key);
create index if not exists in_document_permission_01 on document_permission(holder_id);
create index if not exists in_document_permission_02 on document_permission(document_id);
