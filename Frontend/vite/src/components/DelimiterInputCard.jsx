import { Input, Card, Typography } from "antd";

export default function DelimiterInput({delimiter, setDelimiter}) {
  return (
    <Card style={{ marginTop: 16 }}>
      <Typography.Title level={5} style={{ marginTop: 16 }}>
        Разделитель:
      </Typography.Title>
      <Input
        placeholder="Введите разделитель"
        style={{ width: 200 }}
        value={delimiter}
        onChange={(e) => setDelimiter(e.target.value)}
      />
    </Card>
  );
}
