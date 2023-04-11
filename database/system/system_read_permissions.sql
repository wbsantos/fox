create or replace function fox_system_read_permission_v1(_holder_id uuid)
returns table (permission varchar(255))
language plpgsql as
$$
begin
	return query 
		select
			s1.permission
		from system_permission s1
		where 
			s1.holder_id = _holder_id;
end
$$;