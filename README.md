# Source Mod Pawn Compiler Plugin Helper
---
RU: Помощник для компиляции SourceMod плагинов. 

EN: Helper for compiling Sourcemod plugins

1. Параметры коммандой строки запуска
SMcompiler <path\file.sp>, где <path\file.sp> - имя файла с исходным кодом. 

2.Файловой структуры проекта плагина
1. Только один файл.Т.е. в проекте используется только один файл в формате *.sp.
2. Файловая структура проекта плагина имеет следующий формат:
```sh

<git folder> 
 ├── game<plugin_folder>       # `` например: dod,css,csgo
    ├── addons
       ├── sourcemod
 └── mycmp.ini                 # `параметры компилятора`
```

3. Конфигурация работы помощника
Файл mycmp.ini 
1. В папке с MySMcompiler.
2. В корневой папке проекта плагина.
```sh
[Compiler]
Compilator="spcomp.exe" 
Compilator_Folder="..\smk64t\sourcemod-1.7.3-git5301\"
Plugin_Author="Plugin_Author"
;Parameters=
Include=..\smk64t\scripting\include;..\smk64t\sourcemod-1.7.3-git5301\include;..\smk64t\smlib\scripting\include;
;Always included:
;sourcemod\include 
;smK64t\scripting\include
;smlib\scripting\include

[Server]
rcon_address=ip-address
rcon_port=port
rcon_password="password"
SRCDS_Folder=path to folder in LAN
SRCDS_FTP=path to game folder over ftp server
```



##Changelog 
* Unreleased 
- fix rcon bug
* 1.0.5.3  
-  Autoclose app 
* 1.0.5.0  
 - Added.двойные кавычки в пути -D.  
* 1.0

Use hard datetime format in datetime.inc to prevent locale isses  like "Map_Elections" (╨Я╤В, 15.╨░╨┐╤А.2016 16:00:14) by KOM64T. 
Установлен жесткий формат даты и времени для избежания проблем с форматом локализации.

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
