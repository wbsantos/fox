create or replace function fox_document_create_v1 (
			_stamp_id int,
			_file_binary bytea,
			_file_name varchar(255),
			_size_bytes int
)
returns uuid
language plpgsql as
$$
declare _document_id uuid;
begin
	insert into document
	(
		file_binary,
		stamp_id,
		name,
		file_size_bytes
    )
	values
	(
		_file_binary,
		_stamp_id,
		_file_name,
		_size_bytes
    ) returning id into _document_id;
   
	return _document_id;
end
$$