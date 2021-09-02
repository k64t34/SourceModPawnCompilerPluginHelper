# Source Mod Pawn Compiler Plugin Helper SMcompiler.exe
---
EN: Application is Helper for compiling Sourcemod plugins

Source Mod Pawn Compiler Plugin Helper performs the following functions:
  
- Starts the compilation of the plugin;

- Copies to the server the compiled plugin file and other plugin files (sounds, translation of phrases, etc.);

- Restarts the plugin on the server.

Details in the [wiki](https://github.com/k64t34/SourceModPawnCompilerPluginHelper/wiki)

RU: Source Mod Pawn Compiler Plugin Helper (далее Помощник) выполняет слудующие функции: 
- Запускает компиляцию плагина;

- Копирует на сервер скомпилированный файл плагина и прочие файлы плагина (звуки, перевод фраз и т.п.) ;

- Перезапускает плагин на сервере и(или) TODO:перезапускает карту и(или) TODO:перезапускает сервер.

- TODO: Открывает лог работы сервера, лог ошибок SourceMod

- TODO: Позволяет отправлять команды на сервер

Подробности использования в [Wiki](https://github.com/k64t34/SourceModPawnCompilerPluginHelper/wiki)

##Changelog 
* Unreleased 
 - fix rcon bug
* 1.0.5.3  
 -  Autoclose app 
* 1.0.5.0  
 - Added.двойные кавычки в пути -D.  
* 1.0
 - Use hard datetime format in datetime.inc to prevent locale charset isses  like "Map_Elections" (╨Я╤В, 15.╨░╨┐╤А.2016 16:00:14). 
 -Установлен жесткий формат даты и времени для избежания проблем с кодировкой.

Add parameter MapReload. If MapReload=true, then server will _restart after plugin copy to server. 

В ini добавлен параметр MapReload. Если MapReload установить в true, то сервер будет перезагржен после копирования плагина на сервер.  

Add show plugin info after restart plugin in server.

* 0.3 1st Realese


##Plans

- [x] Удалять datetime.inc после компиляции
- [ ] Добавить в INI пункт сохранять или удалаять .err
- [ ] Добавить фильтр для исключения файлов для копирования на сервер
- [ ] Распозонвание простой структуры плагина
- [ ] need test  Наладить распознование абсолютных и относительных путей 
- [ ] Отображать как абсольные так и относительные пути
- [ ] Получать лог с сервера для слежения за плагинами 
- [ ] Добавить консоль сервера
- [x] При отсутствии действий в окне - закрыть через таймаут
- 


 
https://help.github.com/articles/basic-writing-and-formatting-syntax/
http://keepachangelog.com/ru/
Added для новых функций.
Changed для изменений в существующей функциональности.
Deprecated для функциональности, которая будет удалена в следующих версиях.
Removed для функциональности, которая удалена в этой версии.
Fixed для любых исправлений.
Security 

Usage: MySMcompiler <path\file.sp>
