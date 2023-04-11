create or replace procedure fox_document_delmetadata_v1 (
			_document_id uuid,
			_keys varchar(255)[]
)
language plpgsql as
$$
begin

	delete from document_metadata m
	using unnest(_keys) as _key
	where
		m.key = _key
		and m.document_id = _document_id;

end
$$;