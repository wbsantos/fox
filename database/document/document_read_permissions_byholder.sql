create or replace function fox_document_read_permissions_byholder_v1(_document_id uuid, _holder_id uuid)
returns table (
	permission varchar(255)
)
language plpgsql as
$$
begin
	return query 
		select
			p1.permission
		from document_permission p1
		where
			p1.document_id = _document_id
			and p1.holder_id  = _holder_id
		union
		select
			p2.permission
		from user_group u
		inner join document_permission p2 on p2.holder_id  = u.group_id 
		where
			u.user_id = _holder_id;
end
$$;