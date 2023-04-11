create or replace function fox_group_read_users_v1(_group_id uuid)
returns table (
	id uuid,
	email varchar(255),
	login varchar(63),
	name varchar(255)
)
language plpgsql as
$$
begin
	return query 
		select
			u.id,
			u.email,
			u.login,
			u.name
		from user_account u
		inner join user_group ug on
			ug.group_id = _group_id
			and ug.user_id = u.id;
end
$$;