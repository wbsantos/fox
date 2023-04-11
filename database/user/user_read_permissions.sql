create or replace function fox_user_read_permission_v1(_user_id uuid)
returns table (permission varchar(255))
language plpgsql as
$$
begin
	return query 
		select
			s1.permission
		from system_permission s1
		where 
			s1.holder_id = _user_id
		union
		select
			s2.permission
		from user_group u
		inner join system_permission s2  on s2.holder_id = u.group_id
		where
			u.user_id = _user_id;
end
$$;