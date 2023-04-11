create or replace procedure fox_user_update_v1(
			_id uuid,
			_email varchar(255),
			_login varchar(63),
			_name varchar(255))
language plpgsql as
$$
begin
	update user_account
	set
		email = _email,
		login = _login,
		name = _name
	where
		id = _id;
end
$$;