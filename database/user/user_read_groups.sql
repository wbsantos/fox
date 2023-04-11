create or replace function fox_user_read_groups_v1(_user_id uuid)
returns table (
	id uuid,
	name varchar(255)
)
language plpgsql as
$$
begin
	return query 
		select
			g.id, 
			g.name
		from group_account g
		inner join user_group ug on
			ug.group_id = g.id
			and ug.user_id = _user_id;
end
$$;