# Source Mod Pawn Compiler Plugin Helper SMcompiler.exe
---
```EN```: Application is Helper for compiling [Sourcemod](https://www.sourcemod.net) plugins.

Source Mod Pawn Compiler Plugin Helper performs the following functions:
  
- Starts the compilation of the plugin;

- Copies to the Source Dedicated Serverserver the compiled plugin file and other plugin files (sounds, translation of phrases, etc.);

- Restarts the plugin on the server.

Details in the [wiki](https://github.com/k64t34/SourceModPawnCompilerPluginHelper/wiki).

```RU```: Source Mod Pawn Compiler Plugin Helper (далее Помощник) выполняет слудующие функции: 
- Запускает компиляцию плагина [Sourcemod](https://www.sourcemod.net);

- Копирует на сервер скомпилированный файл плагина и прочие файлы плагина (звуки, перевод фраз и т.п.) ;

- Перезапускает плагин на сервере и(или) TODO:перезапускает карту и(или) TODO:перезапускает сервер.

- TODO: Открывает лог работы сервера, лог ошибок SourceMod

- TODO: Позволяет отправлять команды на сервер

Подробности использования в [Wiki](https://github.com/k64t34/SourceModPawnCompilerPluginHelper/wiki).

## Changelog 
* Unreleased 

- [ ] Create release zip file
- [ ] Fix Restart map
- [ ] Add deleting file  datetime.inc after compiling
- [ ] Add parameter to INI file to 
- [ ] Add a parameter to the INI file to determine whether to save or delete the ERR file
- [ ] Fix rcon bug
- [ ] Add Проверять тип файла SP. Если не SP не запускать компилято
- [ ] Add Добавить фильтр для исключения файлов для копирования на сервер.Не копировать на сервер папку scripting. Не копировать на сервер типы файлов err, bak.
- [ ] Add parameter MapReload. If MapReload=true, then server will sm_map <current_map> after plugin copy to server.
- [ ] Add parameter ServerReload. If ServerReload=true, then server will _restart after plugin copy to server. 
- [ ] Add отображать как абсольные так и относительные пути
- [ ] Fix распознование абсолютных и относительных путей 
- [ ] Add Получать лог с сервера для слежения за плагинами 
- [ ] Add Добавить дублирование консоли сервера
- [ ] Add Изменить проверку файла, если указан не полный путь к файлу а тольк имя или относительный путь
- [ ] Fix Если Compilator_Folder не содержит в начале строки с:\ или \ или \\, то дополнить путь PluginFolder	Compilator_Folder=INIFolder+Compilator_Folder;
- [ ] Fix Check Parameters for copy plugin files to Source Dedicated Server over SMB share or local: hostname, SRCDS_Folder,Share,Share_User,Share_Password
* 6.2021
- [x] Change structure INI file 
* 1.0.5.3  
- [x] Added autoclose app 
* 1.0
- [x] Changed hard datetime format in datetime.inc to prevent locale charset isses  like "Map_Elections" (╨Я╤В, 15.╨░╨┐╤А.2016 16:00:14).  
    Установлен жесткий формат даты и времени для избежания проблем с кодировкой.
- [x] Added show plugin info after restart plugin in server.

