create or replace function fox_document_read_information_v1(_document_id uuid)
returns table (
	id uuid,
	name varchar(255),
	file_size_bytes bigint
)
language plpgsql as
$$
begin
	return query
		select
			d.id,
			d.name,
			d.file_size_bytes 
		from document d 
		where d.id = _document_id;
end
$$;