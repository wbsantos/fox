create or replace procedure fox_user_update_password_v1(
			_id uuid,
			_password bytea,
			_salt bytea,
			_hash_method int)
language plpgsql as
$$
begin
	update user_account
	set
		password = _password,
		salt = _salt,
		hash_method = _hash_method
	where
		id = _id;
end
$$;