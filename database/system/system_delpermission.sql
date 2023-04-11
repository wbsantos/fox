create or replace procedure fox_system_delpermission_v1 (
			_holder_id uuid,
			_permission varchar(255)
)
language plpgsql as
$$
begin
	delete from system_permission
	where
		holder_id = _holder_id
		and permission = _permission;
end
$$;