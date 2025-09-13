import { Input, InputNumber, DatePicker, Space, Radio } from "antd";

export const getTypeComponents = {
  String: (card, setColumnsCards) => (
    <Input
      placeholder="Содержит"
      value={card.Contains || ""}
      onChange={(e) =>
        setColumnsCards((prev) =>
          prev.map((c) =>
            c.id === card.id ? { ...c, Contains: e.target.value } : c
          )
        )
      }
    />
  ),
  Date: (card, setColumnsCards) => (
    <Space>
      <DatePicker
        placeholder="От"
        value={card.DateFrom}
        onChange={(date) =>
          setColumnsCards((prev) =>
            prev.map((c) => (c.id === card.id ? { ...c, DateFrom: date } : c))
          )
        }
      />
      <DatePicker
        placeholder="До"
        value={card.DateTo}
        onChange={(date) =>
          setColumnsCards((prev) =>
            prev.map((c) => (c.id === card.id ? { ...c, DateTo: date } : c))
          )
        }
      />
    </Space>
  ),
  Decimal: (card, setColumnsCards) => (
    <Space>
      <InputNumber
        placeholder="Min"
        value={card.MinValue}
        onChange={(val) =>
          setColumnsCards((prev) =>
            prev.map((c) => (c.id === card.id ? { ...c, MinValue: val } : c))
          )
        }
      />
      <InputNumber
        placeholder="Max"
        value={card.MaxValue}
        onChange={(val) =>
          setColumnsCards((prev) =>
            prev.map((c) => (c.id === card.id ? { ...c, MaxValue: val } : c))
          )
        }
      />
    </Space>
  ),
  Double: (card, setColumnsCards) => (
    <Space>
      <InputNumber
        placeholder="Min"
        value={card.MinValue}
        onChange={(val) =>
          setColumnsCards((prev) =>
            prev.map((c) => (c.id === card.id ? { ...c, MinValue: val } : c))
          )
        }
      />
      <InputNumber
        placeholder="Max"
        value={card.MaxValue}
        onChange={(val) =>
          setColumnsCards((prev) =>
            prev.map((c) => (c.id === card.id ? { ...c, MaxValue: val } : c))
          )
        }
      />
    </Space>
  ),
  Bool: (card, setColumnsCards) => (
    <Radio.Group
      value={card.BoolValue !== undefined ? card.BoolValue : null}
      onChange={(e) =>
        setColumnsCards((prev) =>
          prev.map((c) =>
            c.id === card.id ? { ...c, BoolValue: e.target.value } : c
          )
        )
      }
    >
      <Radio value={true}>Да</Radio>
      <Radio value={false}>Нет</Radio>
    </Radio.Group>
  ),
};
