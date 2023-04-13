create or replace function fox_document_read_information_all_v1(_user_id uuid)
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
			d1.id,
			d1.name,
			d1.file_size_bytes 
		from document d1
		inner join document_permission p1 on
			p1.document_id = d1.id
			and p1.holder_id = _user_id
		union
		select
			d2.id,
			d2.name,
			d2.file_size_bytes 
		from document d2
		inner join user_group ug on
			ug.user_id = _user_id
		inner join document_permission p2 on
			p2.document_id = d2.id
			and p2.holder_id = ug.group_id;
end
$$;