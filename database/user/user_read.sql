create or replace function fox_user_read_v1(_user_login varchar(63))
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
		where 
			u.login = _user_login;
end
$$;