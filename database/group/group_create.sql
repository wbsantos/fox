create or replace function fox_group_create_v1 (_name varchar(255))
returns uuid
language plpgsql as
$$
declare _holder_id uuid;
begin
	insert into holder default values returning id into _holder_id;
    insert into group_account (id, name) values (_holder_id, _name);
   
   return _holder_id;
end
$$;