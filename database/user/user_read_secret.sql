create or replace function fox_user_read_secret_v1(_userlogin varchar(63))
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
			u.login = _userlogin;
end
$$;

--drop function fox_user_read_secret_v1