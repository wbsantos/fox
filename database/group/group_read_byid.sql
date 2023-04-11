create or replace function fox_group_read_byid_v1(_id uuid)
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
		where 
			g.id = _id;
end
$$;