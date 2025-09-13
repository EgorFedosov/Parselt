import { Input, Button } from "antd";

export default function FileSearch({ value, onChange, onSearch }) {
  return (
    <>
      <Input
        placeholder="Введите имя файла для поиска"
        value={value}
        onChange={onChange}
        onPressEnter={onSearch}
        style={{ width: 200, marginRight: 8 }}
      />
      <Button type="primary" onClick={onSearch}>
        Найти
      </Button>
    </>
  );
}
