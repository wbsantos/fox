create or replace function fox_stamp_create_v1 (
			_user_id uuid,
			_system_version varchar(255)
) returns integer 
language plpgsql as
$$
declare _stamp_id int;
begin
	
	insert into stamp (user_id, system_version, created_at)
	values (_user_id, _system_version, now()) 
	returning id into _stamp_id;

	return _stamp_id;
end
$$;