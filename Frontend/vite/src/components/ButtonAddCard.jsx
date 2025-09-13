import { Button } from "antd";
export default function ButtonAddCard({ addCard }) {
  return (
    <Button
      style={{ marginTop: 16 }}
      type="dashed"
      block
      size="large"
      onClick={addCard}
    >
      Добавить столбец
    </Button>
  );
}
