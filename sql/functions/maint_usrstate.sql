
-- 更新、挿入によりuserstate_tblを更新する関数とトリガ
--
CREATE OR REPLACE FUNCTION maint_userstate() RETURNS TRIGGER
AS $maint_userstate$
    DECLARE
        delta_userid          	text;
        delta_state		smallint;
    BEGIN
        -- 増加または減少量を算出
        IF (TG_OP = 'DELETE') THEN

        ELSIF (TG_OP = 'UPDATE') THEN
            -- useridを変更する更新を禁止します
            -- IF ( OLD.userid != NEW.userid) THEN
            --    RAISE EXCEPTION 'Update of userid : % -> % not allowed',OLD.userid, NEW.userid;
            -- END IF;

            delta_userid = OLD.userid;
            IF (NEW.punchintimestamp is not NULL AND NEW.punchouttimestamp is NULL) THEN
		delta_state = 1;
	    ELSE
		delta_state = 2;
	    END IF;
            
        ELSIF (TG_OP = 'INSERT') THEN
            delta_userid = NEW.userid;
            IF (NEW.punchintimestamp is not NULL AND NEW.punchouttimestamp is NULL) THEN
		delta_state = 1;
	    ELSE
		delta_state = 2;
	    END IF;
	END IF;
        <<insert_update>>
        LOOP
            UPDATE userstate_tbl
            SET state = delta_state 
            WHERE userid = delta_userid;

            EXIT insert_update WHEN found;

            BEGIN
                INSERT INTO userstate_tbl (
                            userid,
                            state)
                    VALUES (
                            delta_userid,
                            delta_state
                           );
                EXIT insert_update;

            EXCEPTION
                WHEN UNIQUE_VIOLATION THEN
		-- 何もしません
            END;
        END LOOP insert_update;

        RETURN NULL;

    END;
$maint_userstate$ LANGUAGE plpgsql;

CREATE TRIGGER maint_userstate
AFTER INSERT OR UPDATE OR DELETE ON attendance_tbl
    FOR EACH ROW EXECUTE FUNCTION maint_userstate();