create or replace function fox_document_read_binary_v1(_id uuid)
returns bytea
language plpgsql as
$$
declare _file_binary bytea;
begin
	
	select 
		file_binary
	into _file_binary
	from document 
	where id = _id ;
	
	return _file_binary;
end
$$;