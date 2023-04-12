create or replace procedure fox_system_delpermission_v1 (
			_holder_id uuid,
			_permission varchar(255)
)
language plpgsql as
$$
declare _stamp_ids int[];
begin
	_stamp_ids := array(
		select stamp_id from system_permission
		where
			holder_id = _holder_id
			and permission = _permission);
	
	delete from system_permission
	where
		holder_id = _holder_id
		and permission = _permission;
	
	delete from stamp s 
	where
		s.id = any(_stamp_ids)
		and not exists(select 1 from system_permission p
					   where
					  		p.stamp_id = s.id);
end
$$;