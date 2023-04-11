create or replace procedure fox_system_addpermission_v1 (
			_stamp_id int,
			_holder_id uuid,
			_permission varchar(255)
)
language plpgsql as
$$
begin
	insert into system_permission (stamp_id, holder_id, permission)
	values (_stamp_id, _holder_id, _permission);
end
$$;