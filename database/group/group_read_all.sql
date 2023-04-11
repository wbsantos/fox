create or replace function fox_group_read_all_v1()
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
		from group_account g;
end
$$;