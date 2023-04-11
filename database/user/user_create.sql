create or replace function fox_user_create_v1 (
			_email varchar(255),
			_login varchar(63),
			_password bytea,
			_salt bytea,
			_hash_method int,
			_name varchar(255))
returns uuid
language plpgsql as
$$
declare _holder_id uuid;
begin
	
	insert into holder default values returning id into _holder_id;

    insert into user_account
	(
		id,
		email,
		login,
		password,
		salt,
		hash_method,
		name
    )
	values
	(
		_holder_id,
		_email,
		_login,
		_password,
		_salt,
		_hash_method,
		_name
    );
   
   return _holder_id;
end
$$;