create or replace procedure fox_document_delpermission_v1 (
			_document_id uuid,
			_holder_id uuid,
			_permission varchar(255)
)
language plpgsql as
$$
declare _stamp_ids int[];
begin
	_stamp_ids := array(
		select stamp_id from document_permission
		where
			document_id = _document_id
			and holder_id = _holder_id
			and permission = _permission);

	delete from document_permission
	where
		document_id = _document_id
		and holder_id = _holder_id
		and permission = _permission;

	delete from stamp s 
	where 
		s.id = any(_stamp_ids)
		and not exists(select 1 from document_permission d 
					   where d.stamp_id = s.id);
end
$$;