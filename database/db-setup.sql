--create database fox
/* 
// To recreate all tables, they can be dropped using the commands bellow
drop table DocumentPermission;
drop table DocumentMetadata;
drop table Document;
drop table SystemPermission;
drop table UserGroup;
drop table Stamp;
drop table UserAccount;
drop table GroupAccount;
drop table Holder;
*/

create table if not exists holder --it's either an user or a group id
(
	id uuid default gen_random_uuid() not null,

	CONSTRAINT PK_HOLDER PRIMARY KEY(id)
);

create table if not exists group_account
(
	id uuid default gen_random_uuid() not null,
	name varchar(255) not null,

	CONSTRAINT PK_GROUPACCOUNT PRIMARY KEY(id),
	CONSTRAINT FK_GROUPACCOUNT_HOLDER FOREIGN KEY (id) REFERENCES holder(id)
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

	CONSTRAINT PK_USERACCOUNT PRIMARY KEY(id),
	CONSTRAINT FK_USERACCOUNT_HOLDER FOREIGN KEY (id) REFERENCES holder(id)
);

create table if not exists stamp
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	user_id uuid,
	system_version varchar(255),
	created_at timestamptz,

	CONSTRAINT PK_STAMP PRIMARY KEY(id),
	CONSTRAINT FK_STAMP_USERACCOUNT FOREIGN KEY (user_id) REFERENCES user_account(id)
);

create table if not exists user_group
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	user_id uuid not null,
	group_id uuid not null,
	stamp_id int not null,

	CONSTRAINT PK_USERGROUP PRIMARY KEY(id),
	CONSTRAINT FK_USERGROUP_USERACCOUNT FOREIGN KEY (user_id) REFERENCES user_account(id),
	CONSTRAINT FK_USERGROUP_GROUPACCOUNT FOREIGN KEY (group_id) REFERENCES group_account(id)
	CONSTRAINT FK_USERGROUP_STAMP FOREIGN KEY (stamp_id) REFERENCES stamp(id)	
);

create table if not exists system_permission
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	stamp_id int not null,
	holder_id uuid not null,
	permission varchar(255) not null,

	CONSTRAINT PK_SYSTEMPERMISSION PRIMARY KEY(id),
	CONSTRAINT FK_SYSTEMPERMISSION_HOLDER FOREIGN KEY (holder_id) REFERENCES holder(id),
	CONSTRAINT FK_SYSTEMPERMISSION_STAMP FOREIGN KEY (stamp_id) REFERENCES stamp(id)	
);

create table if not exists document
(
	id uuid default gen_random_uuid() not null,
	stamp_id int not null,
	name varchar(255),
	file_binary bytea not null,
	file_size_bytes int,

	CONSTRAINT PK_DOCUMENT PRIMARY KEY(id),
	CONSTRAINT FK_DOCUMENT_STAMP FOREIGN KEY (stamp_id) REFERENCES stamp(id)	
);

create table if not exists document_metadata
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	document_id uuid not null,
	key varchar(255) not null,
	value varchar(1023),

	CONSTRAINT PK_DOCUMENTMETADATA PRIMARY KEY(id),
	CONSTRAINT FK_DOCUMENTMETADATA_DOCUMENT FOREIGN KEY (document_id) REFERENCES document(id)
);

create table if not exists document_permission
(
	id int GENERATED ALWAYS AS IDENTITY not null,
	stamp_id int not null,
	holder_id uuid not null, 
	document_id uuid not null,
	permission varchar(255) not null,

	CONSTRAINT PK_DOCUMENTPERMISSION PRIMARY KEY(id),
	CONSTRAINT FK_DOCUMENTPERMISSION_HOLDER FOREIGN KEY (holder_id) REFERENCES holder(id),
	CONSTRAINT FK_DOCUMENTPERMISSION_DOCUMENT FOREIGN KEY (document_id) REFERENCES document(id),
	CONSTRAINT FK_DOCUMENTPERMISSION_STAMP FOREIGN KEY (stamp_id) REFERENCES stamp(id)
);

--CREATING INDEXES
CREATE UNIQUE INDEX UN_USERACCOUNT_01 ON user_account(login);
CREATE UNIQUE INDEX UN_GROUPACCOUNT_01 ON group_account(name);
CREATE UNIQUE INDEX UN_USERGROUP_01 ON user_group(groupId, userId);
CREATE INDEX IN_STAMP_01 ON stamp(user_id);
CREATE INDEX IN_STAMP_02 ON stamp(created_at);
CREATE INDEX IN_USERGROUP_01 ON user_group(user_id);
CREATE INDEX IN_USERGROUP_02 ON user_group(group_id);
CREATE INDEX IN_SYSTEMPERMISSION_01 ON system_permission(holder_id);
CREATE INDEX IN_DOCUMENT_01 ON document(name);
CREATE INDEX IN_DOCUMENTMETADATA_01 ON document_metadata(key);
CREATE INDEX IN_DOCUMENTPERMISSION_01 ON document_permission(holder_id);
CREATE INDEX IN_DOCUMENTPERMISSION_02 ON document_permission(document_id);
