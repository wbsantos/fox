create or replace procedure fox_group_adduser_v1 (
			_stamp_id int,
			_group_id uuid,
			_user_ids_to_add uuid[]
)
language plpgsql as
$$
begin
	insert into user_group (group_id, user_id, stamp_id)
	select _group_id, _user_id, _stamp_id
	from unnest(_user_ids_to_add) as _user_id
	where
		not exists(select 1 from user_group u
				   where
				   		u.group_id = _group_id
				   		and u.user_id = _user_id);
end
$$;