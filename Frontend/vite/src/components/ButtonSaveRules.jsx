import { Button } from "antd";

export default function ButtonSaveRules({handleSaveRulesClick}){
    return (
      <Button
        type="primary"
        size="large"
        style={{ float: "right", minWidth: 100, marginTop: 30 }}
        onClick={handleSaveRulesClick}
      >
        Сохранить правила
      </Button>
    );
}