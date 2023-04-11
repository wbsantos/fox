create or replace procedure fox_document_delpermission_v1 (
			_document_id uuid,
			_holder_id uuid,
			_permission varchar(255)
)
language plpgsql as
$$
begin
	delete from document_permission
	where
		document_id = _document_id
		and holder_id = _holder_id
		and permission = _permission;
end
$$;