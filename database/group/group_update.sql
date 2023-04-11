create or replace procedure fox_group_update_v1(
			_id uuid,
			_name varchar(255))
language plpgsql as
$$
begin
	update group_account set name = _name where id = _id;
end
$$;