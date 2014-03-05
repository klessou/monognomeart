
EXTRA_DIST = 

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = mcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -debug -d:DEBUG
ASSEMBLY = ./bin/Debug/MonoGnomeArt.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = ./bin/Debug

NO_THUMBNAIL_PNG_SOURCE=./src/no-thumbnail.png
MONOGNOMEART_GLADE_SOURCE=./src/monognomeart.glade
ICONE24_PNG_SOURCE=./src/icone24.png

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = mcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
ASSEMBLY = ./bin/Release/MonoGnomeArt.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = ./bin/Release

NO_THUMBNAIL_PNG_SOURCE=./src/no-thumbnail.png
MONOGNOMEART_GLADE_SOURCE=./src/monognomeart.glade
ICONE24_PNG_SOURCE=./src/icone24.png

endif


PROGRAMFILES = \
	$(NO_THUMBNAIL_PNG) \
	$(MONOGNOMEART_GLADE) \
	$(ICONE24_PNG)  

BINARIES = \
	$(MONOGNOMEART)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	src/ArtBackground.cs \
	src/ArtDesktop.cs \
	src/ArtGtkEngine.cs \
	src/ArtGtkGnome.cs \
	src/ArtIcon.cs \
	src/ArtItem.cs \
	src/ArtLoginManager.cs \
	src/ArtSplashScreen.cs \
	src/AssemblyInfo.cs \
	src/Cache.cs \
	src/Conf.cs \
	src/Main.cs \
	src/MainWindow.cs \
	src/Theme.cs \
	src/Tray.cs \
	src/TrayIcon.cs \
	src/XmlTreat.cs 

DATA_FILES = \
	src/icone24.png \
	src/monognomeart.glade \
	src/no-thumbnail.png 

RESOURCES = 

EXTRAS = \
	monognomeart.in 

REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	Mono.Http \
	Mono.Posix \
	System \
	$(GLADE_SHARP_20_LIBS) \
	System.Xml

DLL_REFERENCES = 

NO_THUMBNAIL_PNG = $(BUILD_DIR)/no-thumbnail.png
MONOGNOMEART_GLADE = $(BUILD_DIR)/monognomeart.glade
ICONE24_PNG = $(BUILD_DIR)/icone24.png
MONOGNOMEART = $(BUILD_DIR)/monognomeart

$(NO_THUMBNAIL_PNG): $(NO_THUMBNAIL_PNG_SOURCE)
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

$(MONOGNOMEART_GLADE): $(MONOGNOMEART_GLADE_SOURCE)
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

$(ICONE24_PNG): $(ICONE24_PNG_SOURCE)
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'

$(MONOGNOMEART): monognomeart
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'



$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

include $(top_srcdir)/Makefile.include