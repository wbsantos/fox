create or replace procedure fox_group_deluser_v1 (
			_group_id uuid,
			_user_ids uuid[]
)
language plpgsql as
$$
begin

	delete from user_group g
	using unnest(_user_ids) as _user_id 
	where
		g.user_id = _user_id
		and g.group_id = _group_id;

end
$$;