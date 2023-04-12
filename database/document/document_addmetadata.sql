create or replace procedure fox_document_addmetadata_v1 (
			_document_id uuid,
			_items metadata_dictionary[]
)
language plpgsql as
$$
begin
	delete from document_metadata d
	using unnest(_items) _item
	where 
		d.document_id = _document_id
		and d.key = _item.key;
	
	insert into document_metadata (document_id, key, value)
	select _document_id, _item.key, _item.value
	from unnest(_items) _item;
end
$$;


