create or replace procedure fox_document_addmetadata_v1 (
			_document_id uuid,
			_keys varchar(255)[],
			_values varchar(1023)[]
)
language plpgsql as
$$
begin
	delete from document_metadata
	where 
		document_id = _document_id
		and key = any(_keys);
	
	insert into document_metadata (document_id, key, value)
	select _document_id, _key, _value
	from unnest(_keys) with ordinality as keys (_key, _i)
	join unnest(_values) with ordinality as values (_value, _j) on keys._i = values._j;
end
$$;