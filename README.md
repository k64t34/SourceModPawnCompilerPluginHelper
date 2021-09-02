# Source Mod Pawn Compiler Plugin Helper SMcompiler.exe
---
EN: Application is Helper for compiling [Sourcemod](https://www.sourcemod.net) plugins.

Source Mod Pawn Compiler Plugin Helper performs the following functions:
  
- Starts the compilation of the plugin;

- Copies to the server the compiled plugin file and other plugin files (sounds, translation of phrases, etc.);

- Restarts the plugin on the server.

Details in the [wiki](https://github.com/k64t34/SourceModPawnCompilerPluginHelper/wiki).

RU: Source Mod Pawn Compiler Plugin Helper (далее Помощник) выполняет слудующие функции: 
- Запускает компиляцию плагина [Sourcemod](https://www.sourcemod.net);

- Копирует на сервер скомпилированный файл плагина и прочие файлы плагина (звуки, перевод фраз и т.п.) ;

- Перезапускает плагин на сервере и(или) TODO:перезапускает карту и(или) TODO:перезапускает сервер.

- TODO: Открывает лог работы сервера, лог ошибок SourceMod

- TODO: Позволяет отправлять команды на сервер

Подробности использования в [Wiki](https://github.com/k64t34/SourceModPawnCompilerPluginHelper/wiki).

## Changelog 
* Unreleased 
 - Change structure INI file
 - Add deleting file  datetime.inc after compiling
 - Add parameter to INI file to 
 - Fix rcon bug
 - Add parameter MapReload. If MapReload=true, then server will sm_map <current_map> after plugin copy to server.
 - Add parameter ServerReload. If ServerReload=true, then server will _restart after plugin copy to server. 
* 1.0.5.3  
 -  Added autoclose app 
* 1.0
 - Changed hard datetime format in datetime.inc to prevent locale charset isses  like "Map_Elections" (╨Я╤В, 15.╨░╨┐╤А.2016 16:00:14). 
 - Установлен жесткий формат даты и времени для избежания проблем с кодировкой.
 - Added show plugin info after restart plugin in server.

##Plans

- [ ] Добавить в INI пункт сохранять или удалаять .err
- [ ] Добавить фильтр для исключения файлов для копирования на сервер
- [ ] Распозонвание простой структуры плагина
- [ ] need test  Наладить распознование абсолютных и относительных путей 
- [ ] Отображать как абсольные так и относительные пути
- [ ] Получать лог с сервера для слежения за плагинами 
- [ ] Добавить консоль сервера
- [x] При отсутствии действий в окне - закрыть через таймаут
