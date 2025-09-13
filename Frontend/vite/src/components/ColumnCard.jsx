import { Card, Space, Input, Form, Select } from "antd";
const { Option } = Select;
export default function ColumnCard({card, setColumnsCards, submitted, typeComponents}) {
  return (
    <Card
      id={`card-${card.id}`}
      key={card.id}
      title={`Столбец ${card.id}`}
      style={{ marginTop: 16 }}
    >
      <Space align="start">
        <Input
          placeholder="Введите имя столбца"
          style={{ width: 150 }}
          value={card.name || ""}
          onChange={(e) =>
            setColumnsCards((prev) =>
              prev.map((c) =>
                c.id === card.id ? { ...c, name: e.target.value } : c
              )
            )
          }
        />

        <Form.Item
          validateStatus={submitted && card.showError ? "error" : ""}
          help={submitted && card.showError ? "Обязательное поле" : ""}
          style={{ marginBottom: 0 }}
        >
          <Select
            style={{ width: 200 }}
            value={card.type || undefined}
            onChange={(value) =>
              setColumnsCards((prev) =>
                prev.map((c) =>
                  c.id === card.id ? { ...c, type: value, showError: false } : c
                )
              )
            }
          >
            <Option value="String">Строка</Option>
            <Option value="Date">Дата</Option>
            <Option value="Decimal">Десятичное число</Option>
            <Option value="Double">Дробное число</Option>
            <Option value="Bool">Логический тип (Да/Нет)</Option>
          </Select>
        </Form.Item>

        {card.type && typeComponents[card.type](card, setColumnsCards)}
      </Space>
    </Card>
  );
}
