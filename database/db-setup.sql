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

	constraint pk_groupaccount primary key(id),
	constraint fk_groupaccount_holder foreign key (id) references holder(id)
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

	constraint pk_useraccount primary key(id),
	constraint fk_useraccount_holder foreign key (id) references holder(id)
);

create table if not exists stamp
(
	id int generated always as identity not null,
	user_id uuid,
	system_version varchar(255),
	created_at timestamptz,

	constraint pk_stamp primary key(id),
	constraint fk_stamp_useraccount foreign key (user_id) references user_account(id)
);

create table if not exists user_group
(
	id int generated always as identity not null,
	user_id uuid not null,
	group_id uuid not null,
	stamp_id int not null,

	constraint pk_usergroup primary key(id),
	constraint fk_usergroup_useraccount foreign key (user_id) references user_account(id),
	constraint fk_usergroup_groupaccount foreign key (group_id) references group_account(id),
	constraint fk_usergroup_stamp foreign key (stamp_id) references stamp(id)	
);

create table if not exists system_permission
(
	id int generated always as identity not null,
	stamp_id int not null,
	holder_id uuid not null,
	permission varchar(255) not null,

	constraint pk_systempermission primary key(id),
	constraint fk_systempermission_holder foreign key (holder_id) references holder(id),
	constraint fk_systempermission_stamp foreign key (stamp_id) references stamp(id)	
);

create table if not exists document
(
	id uuid default gen_random_uuid() not null,
	stamp_id int not null,
	name varchar(255),
	file_binary bytea not null,
	file_size_bytes int,

	constraint pk_document primary key(id),
	constraint fk_document_stamp foreign key (stamp_id) references stamp(id)	
);

create table if not exists document_metadata
(
	id int generated always as identity not null,
	document_id uuid not null,
	key varchar(255) not null,
	value varchar(1023),

	constraint pk_documentmetadata primary key(id),
	constraint fk_documentmetadata_document foreign key (document_id) references document(id)
);

create table if not exists document_permission
(
	id int generated always as identity not null,
	stamp_id int not null,
	holder_id uuid not null, 
	document_id uuid not null,
	permission varchar(255) not null,

	constraint pk_documentpermission primary key(id),
	constraint fk_documentpermission_holder foreign key (holder_id) references holder(id),
	constraint fk_documentpermission_document foreign key (document_id) references document(id),
	constraint fk_documentpermission_stamp foreign key (stamp_id) references stamp(id)
);

--creating indexes
create unique index un_useraccount_01 on user_account(login);
create unique index un_groupaccount_01 on group_account(name);
create unique index un_usergroup_01 on user_group(group_id, user_id);
create index in_stamp_01 on stamp(user_id);
create index in_stamp_02 on stamp(created_at);
create index in_usergroup_01 on user_group(user_id);
create index in_usergroup_02 on user_group(group_id);
create index in_systempermission_01 on system_permission(holder_id);
create index in_document_01 on document(name);
create index in_documentmetadata_01 on document_metadata(key);
create index in_documentpermission_01 on document_permission(holder_id);
create index in_documentpermission_02 on document_permission(document_id);
