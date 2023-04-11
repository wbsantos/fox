create or replace function fox_document_read_metadata_v1(_document_id uuid)
returns table (
	key varchar(255),
	value varchar(1023)
)
language plpgsql as
$$
begin
	return query
		select
			m.key,
			m.value
		from document_metadata m 
		where m.document_id = _document_id;
end
$$;