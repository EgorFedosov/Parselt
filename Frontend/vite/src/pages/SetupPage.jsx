import { getPreview } from "../services/api";
import { useEffect, useState } from "react";
import DelimiterInputCard from "../components/DelimiterInputCard";
import PreviewCard from "../components/PreviewCard";
import ColumnCard from "../components/ColumnCard";
import ButtonAddCard from "../components/ButtonAddCard";
import ButtonSaveRules from "../components/ButtonSaveRules";
import { getTypeComponents } from "../components/TypeComponents";

export default function SetupPage({
  uploadedFile,
  fileId,
  onNextPage,
  onSaveRules,
}) {
  const [previewData, setPreviewData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [submitted, setSubmitted] = useState(false);
  const [delimiter, setDelimiter] = useState("");

  const [columnsCards, setColumnsCards] = useState([
    { id: 1, showError: false },
  ]);
  const addCard = () => {
    setColumnsCards((prev) => [
      ...prev,
      { id: prev.length + 1, showError: false },
    ]);
  };

  const handleSaveRulesClick = () => {
    setSubmitted(true);
    const firstInvalid = columnsCards.find((card) => !card.type);
    if (firstInvalid) {
      setColumnsCards((prev) =>
        prev.map((c) => (!c.type ? { ...c, showError: true } : c))
      );
      document
        .getElementById(`card-${firstInvalid.id}`)
        ?.scrollIntoView({ behavior: "smooth" });

      return;
    }

    const json = {
      /*На сервер есть возможность загрузить одновременно несколько файлов при одном запросе. Но, в данной версии программы, мы обрабатываем по одному файлу. 
                Сейчас uploadedFile - это всегда массив из 1 элемента  */
      FileId: fileId,
      FileName: Array.isArray(uploadedFile) ? uploadedFile[0] : uploadedFile,
      Delimiter: delimiter,
      Rules: columnsCards.map((card) => {
        const rule = { ColumnName: card.name, DataType: card.type };
        switch (card.type) {
          case "String":
            rule.Contains = card.Contains ?? null;
            break;
          case "Decimal":
          case "Double":
            rule.MinValue = card.MinValue ?? null;
            rule.MaxValue = card.MaxValue ?? null;
            break;
          case "Date":
            rule.DateFrom = card.DateFrom || null;
            rule.DateTo = card.DateTo || null;
            break;
          case "Bool":
            rule.BoolValue = card.BoolValue ?? null;
            break;
        }
        return rule;
      }),
    };
    console.log(JSON.stringify(json, null, 2));
    onSaveRules(json);
    onNextPage();
  };

  useEffect(() => {
    if (!uploadedFile) return;
    const fetchPreview = async () => {
      setLoading(true);
      try {
        const res = await getPreview(uploadedFile);
        setPreviewData(res.data);
      } catch (e) {
        console.log("Ошибка предпросмотра файла: " + e);
      } finally {
        setLoading(false);
      }
    };
    fetchPreview();
  }, [uploadedFile]);

  const columns = [
    {
      title: uploadedFile,
      dataIndex: "rawRow",
      key: "rawRow",
    },
  ];

  const dataSource =
    previewData?.rows?.map((row, index) => ({
      key: index,
      rawRow: row.rawRow,
    })) || [];

  return (
    <>
      {
        <PreviewCard
          loading={loading}
          dataSource={dataSource}
          columns={columns}
        />
      }
      {!loading && previewData && (
        <>
          {
            <DelimiterInputCard
              delimiter={delimiter}
              setDelimiter={setDelimiter}
            />
          }

          {columnsCards.map((card) => (
            <ColumnCard
              card={card}
              setColumnsCards={setColumnsCards}
              submitted={submitted}
              typeComponents={getTypeComponents}
            />
          ))}

          <ButtonAddCard addCard={addCard} />
          <ButtonSaveRules handleSaveRulesClick={handleSaveRulesClick} />
        </>
      )}
    </>
  );
}
