create or replace function fox_document_read_permissions_bydoc_v1(_document_id uuid)
returns table (
	holder_id uuid,
	holderlogin varchar(63),
	holdername varchar(255),
	permission varchar(255),
	holdertype varchar(10)
)
language plpgsql as
$$
begin
	return query 
		select
			p1.holder_id,
			u.login,
			u.name,
			p1.permission,
			'user'::varchar(10) as holdertype
		from document_permission p1
		inner join user_account u on u.id = p1.holder_id
		where
			p1.document_id = _document_id
		union
		select
			p2.holder_id,
			''::varchar(63),
			g.name,
			p2.permission,
			'group'::varchar(10) as holdertype
		from document_permission p2
		inner join group_account g on g.id = p2.holder_id
		where
			p2.document_id = _document_id;

end
$$;