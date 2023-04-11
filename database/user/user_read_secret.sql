create or replace function fox_user_read_secret_v1(_user_login varchar(63))
returns table (
	id uuid,
	password bytea,
	salt bytea,
	hash_method int
)
language plpgsql as
$$
begin
	return query 
		select
			u.id,
			u.password,
			u.salt,
			u.hash_method
		from user_account u
		where 
			u.login = _user_login;
end
$$;

--drop function fox_user_read_secret_v1