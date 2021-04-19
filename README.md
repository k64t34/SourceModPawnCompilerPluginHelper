# Source Mod Pawn Compiler Plugin Helper
---
SMcompiler.exe

RU: Помощник для компиляции SourceMod плагинов. 

EN: Helper for compiling Sourcemod plugins

0.Перед запуском SMcompiler.exe должена существовать папка SourceMod со всей файловой структурой.
Оптимально размещать файл SMcompiler.exe и smcmphlp.ini в папке ...\SOURCEMOD\addons\sourcemod\scripting
В дальнейшем описании для примера будем использовать папку D:\USERS\<UserName>\PROJECT внутри которой будут папки с проектами.

```sh

D:\USERS\<UserName>\PROJECT\SOURCEMOD
├───addons
│   ├───metamod
│   └───sourcemod
│       ├───bin
│       ├───configs
│       │   ├───geoip
│       │   └───sql-init-scripts
│       │       ├───mysql
│       │       └───sqlite
│       ├───data
│       ├───extensions
│       ├───gamedata
│       │   ├───core.games
│       │   ├───sdkhooks.games
│       │   ├───sdktools.games
│       │   └───sm-cstrike.games
│       ├───logs
│       ├───plugins
│       │   └───disabled
│       ├───scripting
│       │   ├───admin-flatfile
│       │   ├───adminmenu
│       │   ├───basebans
│       │   ├───basecomm
│       │   ├───basecommands
│       │   ├───basevotes
│       │   ├───funcommands
│       │   ├───funvotes
│       │   ├───include
│       │   ├───playercommands
│       │   └───testsuite
│       └───translations
│           ├───ar
│           :
│           └───zho
└───cfg
    └───sourcemod
```
При использовании библиотеки smK64t  также необходимо клонировать https://github.com/k64t34/smK64t.sourcemod с подмодулем smLib https://github.com/bcserv/smlib

В итоге должно быть три папки

D:\USERS\<UserName>\PROJECT
                      ├─ SOURCEMOD
                      ├─ SMK64T.SOURCEMOD 
                      └─ PLUGIN


1. Параметры коммандой строки SMcompiler.exe
SMcompiler.exe <path\plugin.sp>, где <path\plugin.sp> - имя файла с исходным кодом. 

2.Файловая структура проекта плагина или только один файл
1. Только один файл.т.е. в папке проекта используется только один файл в формате *.sp
2. Файловая структура проекта плагина имеет следующий формат:
```sh

D:\USERS\<UserName>\PROJECT\PLUGIN
 ├── GAMEMOD
 │   ├── ADDONS  
 │   │   ├── SOURCEMOD
 │   │       ├── [TRANSLATIONS]
 │   │       │     ├── [plugin.phrases.txt]
 │   │       │     ├── [RU]
 │   │       │     │    └── [plugin.phrases.txt]
 │   │       │     :
 │   │       └── SCRIPTING
 │   │             ├── [INCLUDE]
 │   │             │    ├───── [file1.inc]
 │   │             │    ├───── [file2.inc]
 │   │             │    :
 │   │             └─ plugin.sp #` исходный файл 
 │   ├── [CFG] 
 │   │    └── [SOURCEMOD] 
 │   │         └── [plugin.cfg]
 │   ├── [DOWNLOAD] 
 │   │    ├── [MAPS]
 │   │    ├── [MODELS]
 │   │    ├── [MATERIALS]
 │   │    ├── [RESOURCE]
 │   │    │   ├── [OVERVIEWS]
 │   │    │   : 
 │   │    ├── [SOUND]
 │   │    ├── [USER_CUSTOM]
 │   ├── [CUSTOM] 
 │        ├── [PLUGIN]
 │             ├── [MODELS]
 │             │    ├───── [plugin_model1.mdl]
 │             │    ├───── [plugin_model2.mdl]
 │             │    :
 │             ├── [MATERIALS]
 │             │    ├───── [plugin_material1.vmt]
 │             │    ├───── [plugin_material2.vmt]
 │             │    :
 │             ├── [VGUI]
 │             │    ├───── [plugin_ui_thing.res]
 │             └── [plugin.vpk] 	
 └── smcmphlp.ini                # `параметры компилятора`
 
 
 
```

3. Конфигурация работы помощника SMcompiler.exe
Файл smcmphlp.ini 

SMcompiler.exe сначала читает файл smcmphlp.ini из той же папки что и он сам, затем из корневой папки проекта плагин. 
Если в разных файлах smcmphlp.ini встречается один и тот же параметр с разными значениями, то параметр перезаписыватся из последнего прочитанного файла.

```sh

Описания параметров в файле smcmphlp.ini

Пути к файлам и папкам можно указывать как асболютный так и относительный.
Точкой отсчета относительного пути к файлу или папке явлется папка текущего проекта.
Пример указания асболютного пути: 
D:\USERS\<UserName>\PROJECT\SOURCEMOD\addons\sourcemod\scripting 
Пример указания относительного пути: 
..\SOURCEMOD\addons\sourcemod\scripting 

[Compiler]
;Файл компилятора плагинов для SourceMod. 
Compilator="spcomp.exe" 
;Параметры, передаваемы в коммандную строку компилятора плагина
Parameters=

;TODO:make descriptipn
Include=..\smk64t\scripting\include;..\smk64t\sourcemod-1.7.3-git5301\include;..\smk64t\smlib\scripting\include;

;Default included:
;sourcemod\scripting\include 

;TODO:make descriptipn
;smK64t\scripting\include
;smlib\scripting\include


;Папка где расположен файл компилятора. В данном случае это папка D:\USERS\<UserName>\PROJECT\SOURCEMOD\addons\sourcemod\scripting 
;Можно указывать абсолютный путь. D:\USERS\<UserName>\PROJECT\SOURCEMOD\addons\sourcemod\scripting 
;Можно указывать путь относительно папки текущего проекта. ..\SOURCEMOD\addons\sourcemod\scripting 
Compilator_Folder=..\SOURCEMOD\addons\sourcemod\scripting
;Указать автора плагина
Plugin_Author="Plugin_Author"

; Параметры для отладки плагина на сервере
[Server]
rcon_address=ip-address
rcon_port=port
rcon_password="password"
; Путь куда копировать скомпилированный плагин по протоколу SMB. Путь в формати URI. Это может быть локальный диск или  удаленный сервер
;path to folder in LAN or
SRCDS_Folder=\\server1\c$\srcds\game\addons\sourcemod\plugin
; Путь куда копировать скомпилированный плагин по протоколу FTP. Путь в формати URL.
SRCDS_FTP=ftp://server/game/addons/sourcemod/plugin
;Перезапусить srcds перед отладкой новой скомпилированной версии плугина
RestartServer=False
;Перезапусить карту на srcds  перед отладкой новой скомпилированной версии плугина
RestartMap=No



```

---

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
