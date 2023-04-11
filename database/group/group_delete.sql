CREATE OR REPLACE PROCEDURE fox_group_delete_v1(_id uuid)
LANGUAGE plpgsql AS
$$
BEGIN
	DELETE FROM document_permission WHERE holder_id = _id;
	DELETE FROM system_permission WHERE holder_id = _id;
	DELETE FROM user_group WHERE group_id = _id;

	DELETE FROM group_account WHERE id = _id;
	DELETE FROM Holder WHERE id = _id;
END
$$;