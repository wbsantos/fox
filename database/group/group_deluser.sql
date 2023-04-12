create or replace procedure fox_group_deluser_v1 (
			_group_id uuid,
			_user_ids uuid[]
)
language plpgsql as
$$
declare _stamp_ids int[];
begin
	_stamp_ids := array(
		select g.stamp_id from user_group g 
		where
			g.user_id = any(_user_ids)
			and g.group_id = _group_id
	);

	delete from user_group g
	using unnest(_user_ids) as _user_id 
	where
		g.user_id = _user_id
		and g.group_id = _group_id;

	delete from stamp s 
	where
		s.id = any(_stamp_ids) 
		and not exists(select 1 from user_group u
					   where u.stamp_id = s.id);
end
$$;
