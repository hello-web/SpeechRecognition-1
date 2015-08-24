# SpeechRecognition
Нет содержания и написано довольно-так коряво, но я надеюсь, что понятно
(Д. Бастрич, bastrich@bk.ru)

<b>Распознавание речи<br>
Как пользоваться программой</b>

Интерфейс программы состоит из главной формы, где проводится непосредственно распознавание, выводится список известных слов и для каждого слова выводится график его MFCC. Также на главной форме расположено текстовое поле, куда выводится информация о том, какое слово было распознано, и расстоянии между MFCC-векторами различных слов.
При нажатии кнопки «Добавить» под списком известных слов появляется форма для обучения системы какому-нибудь одному слову. Для обучения требуется три раза произнести слово и нажать кнопку «Сохранить». На форме отображается текущий прогресс. Также возможно начать весь цикл обучения заново или заново записать последнее слово при нажатии на соответствующие кнопки.
Про распознавании и при обучении требуется записывать слова с помощью микрофона. В обоих случаях для этого требуется:
1)	Нажать кнопку с зелёным кружком.
2)	Произнести слово.
3)	Нажать кнопку с красным кружком.
Чем меньше промежутки времени между нажатием кнопок и границами слова, тем лучше, так как в данный момент выделение границ слова не работает должным образом.
	При нажатии кнопки сохранить файл новое слово появляется в списке известных слов на главной форме. Теперь его можно попробовать распознать.
	Все записанные слова, и при обучении, и при распознавании записываются в папку waves, которая создаётся в одной папке с файлом для запуска программы. Именами файлов является время их записи. 
	Слова, которым была обучена система, сохраняются в файле data.dat.
	Также для при каждом запуске программы создаётся или перезаписывается файл log.txt, в который записывается информация о работе текущей сессии (правда сейчас в лог мало что записывается).


<b>Внутреннее устройство и работа</b><br>
Основная логика в данный момент следующая:<br>
Обучение
1)	Несколько раз записывается одно и то же слово. 
2)	Для каждой записи считаются MFCC.
3)	Вектора MFCC для каждой записи одного слова нормализуются, усредняются.
4)	Результирующий вектор записывается в ArrayList известных слов.

Распознавание
1)	Один раз записывается слово.
2)	Считаются его MFCC.
3)	MFCC-вектор нормализуется.
4)	Сравниваются расстояния между MFCC-вектором записанного слова  и каждым из известных слов. Результатом является слово, до MFCC-вектора которого наименьшее расстояние.

<b>Как происходит расчёт MFCC</b><br>
Для записи используется сторонняя библиотека NAudio.
Записанное слово представляет собой ряд чисел, которые представляют собой амплитуды сигнала в определённые моменты времени. Над этими отсчётами производится преобразование Фурье. В результате мы тоже получаем какой-то набор чисел. По-разному интерпретирую результаты преобразования Фурье, мы можем получать спектр различных характеристик сигнала (амплитуда, энергия и т.д.) по частоте. 
Результаты преобразования Фурье содержать действительную и мнимую часть. Для получения MFCC требуется спектр энергии сигнала по частоте. Для этого складываются квадраты действительно и мнимой части для каждого отсчёта. 
Использование MFCC строится на том, как воспринимает частоту сигнала человеческое ухо. Для измерения восприятия человеческим ухом есть специальная величина - Мел, она зависит определённым образом от Герца. Смысл Мела в том, чтобы обращать больше внимания именно на те частоты, которые слышит человеческое ухо. 
Достаточно понятно и более подробно об этом можно почитать, например, здесь:
http://practicalcryptography.com/miscellaneous/machine-learning/guide-mel-frequency-cepstral-coefficients-mfccs/
Грубо говоря, для получения MFCC мы применяем некий фильтр к спектру энергий.
Можно регулировать количество получаемых для сигнала MFCC, тем самым получая более точную информацию. Стандартно рассчитываются 26 коэффициентов и берутся 2-13 коэффициенты. MFCC представляют собой действительные числа.
В итоге мы получаем некий MFCC-вектор. Для более адекватного сравнения мы его нормализуем относительно единицы (хотя лично мне не совсем понятен смысл). 
	
<b>Как происходит распознавание</b> <br>
Распознавание сейчас является простым сравнением расстояний между MFCC-векторами.

<b>Структура кода программы</b><br>
Решение состоит из одного проекта. В свою очередь, проект состоит из следующих классов:
1)	MainSpeech – класс, единственный объект которого создаётся при загрузке главной формы. Содержит ArrayList известных слов, методы для записи/чтения файла data.dat, методы для сравнения векторов.
2)	SoundProcessing – класс, который отвечает за запись, обработку звука(Фурье, MFCC) и т.д. Он использует следующие классы:
1)	WaveInput – класс для записи звука с микрофона или чтения из wave-файла.
2)	RawTransform – класс, в котором производится обработка звука.
3)	FFT – реализация преобразования Фурье.
       3)  LBPlot – отвечает за построение графиков
       4)  Logger – статический класс для записи логов в файл.
       5)  В классах форм MainForm и Learning происходит соответственно распознавание и обучение.

<b>Примечания</b><br>
1)	Распознавание сейчас реализовано очень примитивно, в дальнейшем планируется применение более совершенных алгоритмов.
2)	Сейчас преобразования проводятся над всем сигналом сразу. Но это нам кажется (скорее мы даже в это уверены) не совсем корректным, потому что непонятно в этом случае, что мы получаем (например в результате разложения Фурье), хотя в некоторых статьях также делают. По идее надо брать отрезки по 20-30 мс и смотреть их, а потом как-то строить общую характеристику для слова. Над этим вопросом тоже будем работать.
3)	На главной форме есть кнопка «Обзор». Там можно выбирать wave-файл и для него будут строиться разные красивые графики (а точнее график амплитуд по времени, график амплитуд по частоте и график MFCC, при этом преобразование не над всем словом, а над его частями) . Сейчас это вроде работает, но всё равно есть некоторые незначительные баги.
4)	Класс FFT является сторонней реализацией быстрого преобразования Фурье с помощью рекурсии. Была наша реализация (без рекурсии), но она работал как-то не так, в общем результаты работы класса FFT мы сочли более правильными. Хотя полной уверенности в этом нет.
5)	Сейчас далеко логируется далеко не всё. Но это легко поправимо с помощью вставления в нужные места программы вызова метода Logger.Add(“какая-то информация”). Никакой специальной инициализации для логгера не требуется, всё делается автоматически.
6)	В основном для хранения рядов чисел используется ArrayList, но в классах, связанных непосредственно с обработкой используются массивы. Такое смешение не есть хорошо. Просто это было написано в разное время или разными людьми, поэтому в некоторых  местах может наблюдаться использование разных типов данных для хранения одного и того же.
7)	Сейчас очень много кода, связанного непосредственно с пользовательским интерфейсом. В дальнейшем планируется избавление от этого.
8)	В общем и целом созданная нами архитектура очень далека от совершенства. Во многом это связано с тем, что в начале, когда планировалась архитектура, почти не было представления о том что и как должно было бы работать. Но это тоже планируется исправлять. 