# planCompare
VARIAN社製治療計画装置EclipseのScripting codeです。

実装機能：Clinical Protocolに登録されているプランを比較するツール

作成環境：v15.6

動作検証：v15.6


# 使用方法について
1. 「planCompare.cs」を「%%ARIASERVER%%\va_data$\ProgramData\Vision\PublishedScripts」にコピーする。
　　　注）%%ARIASERVER%%の部分は施設のサーバー名に置き換えてください。

2. Clinical protocolにアタッチされているプランをロードします。

3. メニュー「Tools」→「Scripts」の順で一覧に表示されている「planCompare.cs」を選んで「Run」ボタンを押します。

4. Clinical protocolにアタッチされている全プランのPrescirptionとQualityIndexが一覧表示されます。


# planCompareの表示画面
![Screen capture of planCompare UI](https://github.com/tkmd94/planCompare/blob/master/Screenshot_planCompare.PNG)
