using Structurizr;
using Structurizr.Api;
using Structurizr.Core.Util;
using System;
using System.Linq;

namespace fas_c4_model
{
    class Program
    {
        static void Main(string[] args)
        {
            Banking();
        }
        
        static void Banking()
        {
            const long workspaceId = 76595;
            const string apiKey = "4afcb810-b98f-4b78-a6b0-cb1ec7c21ec5";
            const string apiSecret = "001eb260-1c47-4358-961b-751447b6d86f";

            StructurizrClient structurizrClient = new StructurizrClient(apiKey, apiSecret);
            Workspace workspace = new Workspace("Squirrel's Box - C4 Model", "Sistema de Gestion de Inventario");
            Model model = workspace.Model;

            SoftwareSystem tutoringSystem = model.AddSoftwareSystem(Location.Internal, "Sistema de Gestion de Inventario", "Permite el monitoreo y organización del inventario");
            SoftwareSystem sms = model.AddSoftwareSystem("SmsManager", "Servicio de mensajeria externa");
            SoftwareSystem paypal = model.AddSoftwareSystem("Paypal/visa", "Plataforma que el servicio de medios de pago");
            SoftwareSystem firebase = model.AddSoftwareSystem("Firebase","Plataforma Cloud que otorga servicios de almacenamiento, authentication, entre otros");
            SoftwareSystem cloudVisionApi = model.AddSoftwareSystem("Google Cloud Vision API", "API que permite el reconocimiento de objetos a través de imágenes");

            Person mype = model.AddPerson("Mype Owner", "Persona poseedora de una microempresa o negocio");
            Person admin = model.AddPerson(Location.Internal, "Admin", "Admin - Open Data.");


            mype.Uses(tutoringSystem, "Realiza consultas con respecto a cajas, secciones y objetos de su inventario");
            admin.Uses(tutoringSystem, "Realiza consultas a la REST API para mantenerse al tanto de las compras hechas por los usuarios");

           
            tutoringSystem.Uses(sms, "Se comunica con el usuario para el envio de los mensajes de autenticación");
            tutoringSystem.Uses(paypal, "Se comunica con el sistema para la realizacion de pagos en transacciones");
            tutoringSystem.Uses(firebase, "Se comunica con el sistema para almacenamiento en la nube");
            tutoringSystem.Uses(cloudVisionApi, "Realiza consultas para la identificación de objetos");

            ViewSet viewSet = workspace.Views;


            //---------------------------//---------------------------//
            // 1. System Context Diagram
            //---------------------------//---------------------------//

            SystemContextView contextView = viewSet.CreateSystemContextView(tutoringSystem, "Contexto", "Diagrama de contexto");
            contextView.PaperSize = PaperSize.A4_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            // Tags
            tutoringSystem.AddTags("SistemaGestion");
            sms.AddTags("sms");
            firebase.AddTags("Firebase");
            paypal.AddTags("Paypal");
            mype.AddTags("MypeOwner");
            admin.AddTags("Admin");
            cloudVisionApi.AddTags("Firebase");

            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle("MypeOwner") { Background = "#0a60ff", Color = "#ffffff", Shape = Shape.Person });
            
            styles.Add(new ElementStyle("Admin") { Background = "#facc2e", Shape = Shape.Robot });
            styles.Add(new ElementStyle("SistemaGestion") { Background = "#008f39", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("sms") { Background = "#90714c", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("Paypal") { Background = "#828412", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("Firebase"){Background = "#2196F3", Color = "#ffffff", Shape = Shape.RoundedBox});

            //---------------------------//---------------------------//
            // 2. Conteiner Diagram
            //---------------------------//---------------------------//

            Container mobileApplication = tutoringSystem.AddContainer("Mobile App", "Permite a los usuarios visualizar la aplicacion principal donde el usuario realiza sus operaciones", "Kotlin");
            Container webApplication = tutoringSystem.AddContainer("Web App", "Permite a los usuarios visualizar un dashboard con algunas funcionalidades que brinda la aplicación.", "Vuetify");
            Container landingPage = tutoringSystem.AddContainer("Landing Page", "", "HTML, CSS, Javascript");

            Container apiGateway = tutoringSystem.AddContainer("API Gateway", "API Gateway", "Retrofit/Java");
            SoftwareSystem dbSqlite = model.AddSoftwareSystem("Database", "SQLite \nRegistro de ocurrencias de la aplicacion.");
            SoftwareSystem firebasedb=model.AddSoftwareSystem("Firebase DB","SQL \nRegistro de informacion de la aplicación");

            Container sessionContext = tutoringSystem.AddContainer("Session Bounded Context", "Bounded Context para gestión de sesiones", "Spring Boot port 8085");           
            Container profileContext = tutoringSystem.AddContainer("Profile Bounded Context", "Bounded Context para gestión de membresias", "Spring Boot port 8089");
            Container inventoryContext = tutoringSystem.AddContainer("Inventory Bounded Context", "Bounded Context para gestión de herramientas externas", "Spring Boot port 8082");
            Container subscriptionContext = tutoringSystem.AddContainer("Subscription Bounded Context", "Bounded Context para gestión de pagos", "Spring Boot port 8081");
            

            mype.Uses(mobileApplication, "Consulta");
            mype.Uses(webApplication, "Consulta");
            mype.Uses(landingPage, "Consulta");

            admin.Uses(apiGateway, "API Request", "JSON/HTTPS");

            mobileApplication.Uses(apiGateway, "API Request", "JSON/HTTPS");
            webApplication.Uses(apiGateway, "API Request", "JSON/HTTPS");

            apiGateway.Uses(sessionContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(profileContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(inventoryContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(subscriptionContext, "API Request", "JSON/HTTPS");
            
                        
            sessionContext.Uses(firebasedb, "lee y escribe hasta", "JDBC");
            sessionContext.Uses(dbSqlite, "lee y escribe hasta", "JDBC");
            sessionContext.Uses(sms, "envía solicitud para envío de sms", "JDBC");

            profileContext.Uses(firebasedb, "lee y escribe hasta", "JDBC");

            inventoryContext.Uses(firebasedb, "lee y escribe hasta", "JDBC");
            inventoryContext.Uses(cloudVisionApi, "envía request de reconocimiento de imagen", "JSON");

            subscriptionContext.Uses(paypal, "", "JDBC");
            subscriptionContext.Uses(firebasedb, "lee y escribe hasta", "JDBC");

            sms.Uses(sessionContext, "Envía mensaje de confirmación", "SMS");
            cloudVisionApi.Uses(inventoryContext, "retorna objeto/item reconocido", "JSON");

           
            // Tags
            mobileApplication.AddTags("MobileApp");
            webApplication.AddTags("WebApp");
            landingPage.AddTags("LandingPage");
            apiGateway.AddTags("APIGateway");
            dbSqlite.AddTags("DataBase");
            firebasedb.AddTags("FirebaseDB");

            sessionContext.AddTags("BoundedContext");
            profileContext.AddTags("BoundedContext");
            inventoryContext.AddTags("BoundedContext");
            subscriptionContext.AddTags("BoundedContext");

            


            styles.Add(new ElementStyle("MobileApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.MobileDevicePortrait, Icon = "" });
            styles.Add(new ElementStyle("WebApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("LandingPage") { Background = "#929000", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("APIGateway") { Shape = Shape.RoundedBox, Background = "#0000ff", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("BoundedContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("DataBase") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("FirebaseDB") { Shape = Shape.Cylinder, Background = "#2196F3", Color = "#ffffff", Icon = "" });

            ContainerView containerView = viewSet.CreateContainerView(tutoringSystem, "Contenedor", "Diagrama de contenedores");
            containerView.PaperSize = PaperSize.A3_Landscape;
            containerView.AddAllElements();
            containerView.Remove(firebase);


            //---------------------------//---------------------------//
            // 3. Component Diagrams
            //---------------------------//---------------------------//

            // Components Diagram - Session Bounded Context
            Component sessionFragment = sessionContext.AddComponent("Login Fragment", "Interfaz de loggeo", "");
            Component registerFragment = sessionContext.AddComponent("Register Fragments", "Interfacz de registro en la app", "");
            Component verificationFragment = sessionContext.AddComponent("Verification Fragments", "Interfaz para verificación de número de teléfono", "");

            Component sessionViewModelFragment = sessionContext.AddComponent("Session View Model Fragment", "Provee métodos para realiza el login", "");
            Component registerViewModelFragment = sessionContext.AddComponent("Register View Model Fragment", "Provee métodos para registro de usuario", "");
            Component verificationViewModelFragment = sessionContext.AddComponent("Verification View Model Fragment", "Provee métodos para verificación de teléfono", "");

            Component sessionUserRepository = sessionContext.AddComponent("User Repository", "Provee los métodos para la persistencia de datos de Session", "");
            Component sessionRepository = sessionContext.AddComponent("Session Repository", "Provee los métodos para la persistencia de datos de manera local", "Room Component");

            Component sessionDomainLayer = sessionContext.AddComponent("Session Domain Layer", "Contiene las entidades Core del bounded context", "");


            // Components Diagram - Profile Bounded Context
            Component profileFragment = profileContext.AddComponent("Profile Fragments", "Interfaces del perfil del usuario", "");
            Component settingsFragment = profileContext.AddComponent("Settings Fragments", "Interfaz de Settings del usuario", "");

            Component profileViewModelFragment = profileContext.AddComponent("Profile View Model Fragment", "Provee métodos para manejo del perfil", "");
            Component settingsViewModelFragment = profileContext.AddComponent("Settings View Model Fragment", "Provee métodos para manejo de ajustes", "");

            Component settingsRepository = profileContext.AddComponent("Settings Repository", "Provee los métodos para la persistencia de datos de ajustes", "");
            Component profileRepository = profileContext.AddComponent("Profile Repository", "Provee los métodos para la persistencia de datos del perfil de usuario", "");
            Component membershipRepository = profileContext.AddComponent("Membership Repository", "Provee los métodos para la persistencia de datos de las membresías", "");

            Component profileDomainLayer = profileContext.AddComponent("Profile Domain Layer", "Contiene las entidades Core del bounded context", "");


            // Components Diagram - Subscription Bounded Context
            Component membershipPurchasFragment = subscriptionContext.AddComponent("Membership Purchase Fragments", "Interfaces de membresías", "");
            Component boxPurchaseFragment = subscriptionContext.AddComponent("Box Purchase Fragments", "Interfaz de compra de cajas", "");

            Component membershipPurchasViewModelFragment = subscriptionContext.AddComponent("Membership Purchase View Model Fragment", "Provee métodos para manejo de pagos de mebresía", "");
            Component boxPurchaseViewModelFragment = subscriptionContext.AddComponent("\"Box Purchase View Model Fragment", "Provee métodos para manejo de compras de cajas", "");

            Component membershipPurchaseRepository = subscriptionContext.AddComponent("Membership Purchase Repository", "Provee los métodos para la persistencia de datos de pagos de membresía", "");
            Component boxPurchaseRepository = subscriptionContext.AddComponent("Box Purchase Repository", "Provee los métodos para la persistencia de datos de compras de cajas", "");

            Component subscriptionDomainLayer = subscriptionContext.AddComponent("Subscription Domain Layer", "Contiene las entidades Core del bounded context", "");


            // Components Diagram - Inventory Bounded Context
            Component boxFragment = inventoryContext.AddComponent("Box Fragments", "Interfaces del las cajas", "");
            Component sectionFragment = inventoryContext.AddComponent("Section Fragments", "Interfaz de las secciones", "");
            Component itemFragment = inventoryContext.AddComponent("Item Fragments", "Interfaz de los items", "");

            Component boxViewModelFragment = inventoryContext.AddComponent("Box View Model Fragment", "Provee métodos para manejo de las cajas", "");
            Component sectionsViewModelFragment = inventoryContext.AddComponent("Section View Model Fragment", "Provee métodos para manejo de secciones", "");
            Component itemViewModelFragment = inventoryContext.AddComponent("Item View Model Fragment", "Provee métodos para manejo de items", "");

            Component homeBoxRepository = inventoryContext.AddComponent("Home Box Repository", "Provee los métodos para la persistencia de datos de cajas", "");
            Component sharedBoxRepository = inventoryContext.AddComponent("Shared Box Repository", "Provee los métodos para la persistencia de datos cajas compartidas", "");
            Component homeSectionRepository = inventoryContext.AddComponent("Home Section Repository", "Provee los métodos para la persistencia de datos de secciones", "");
            Component homeItemRepository = inventoryContext.AddComponent("Home Item Repository", "Provee los métodos para la persistencia de datos de items", "");
            Component cloudVisionViewModel = inventoryContext.AddComponent("Cloud Vision View Model", "Provee los métodos para la comunicación con Cloud Vision API", "");

            Component inventoryDomainLayer = inventoryContext.AddComponent("Inventory Domain Layer", "Contiene las entidades Core del bounded context", "");



            // Tags

            sessionFragment.AddTags("Controller");
            registerFragment.AddTags("Controller");
            verificationFragment.AddTags("Controller");
            sessionViewModelFragment.AddTags("Service");
            registerViewModelFragment.AddTags("Service");
            verificationViewModelFragment.AddTags("Service");
            sessionUserRepository.AddTags("Repository");
            sessionRepository.AddTags("Repository");
            sessionDomainLayer.AddTags("Repository");

            profileFragment.AddTags("Controller");
            settingsFragment.AddTags("Controller");
            profileViewModelFragment.AddTags("Service");
            settingsViewModelFragment.AddTags("Service");
            settingsRepository.AddTags("Repository");
            profileRepository.AddTags("Repository");
            membershipRepository.AddTags("Repository");
            profileDomainLayer.AddTags("Repository");

            membershipPurchasFragment.AddTags("Controller");
            boxPurchaseFragment.AddTags("Controller");
            membershipPurchasViewModelFragment.AddTags("Service");
            boxPurchaseViewModelFragment.AddTags("Service");
            membershipPurchaseRepository.AddTags("Repository");
            boxPurchaseRepository.AddTags("Repository");
            subscriptionDomainLayer.AddTags("Repository");

            boxFragment.AddTags("Controller");
            sectionFragment.AddTags("Controller");
            itemFragment.AddTags("Controller");
            boxViewModelFragment.AddTags("Service");
            sectionsViewModelFragment.AddTags("Service");
            itemViewModelFragment.AddTags("Service");
            homeBoxRepository.AddTags("Repository");
            sharedBoxRepository.AddTags("Repository");
            homeSectionRepository.AddTags("Repository");
            homeItemRepository.AddTags("Repository");
            cloudVisionViewModel.AddTags("Repository");
            inventoryDomainLayer.AddTags("Repository");



            styles.Add(new ElementStyle("Controller") { Shape = Shape.Component, Background = "#FDFF8B", Icon = "" });
            styles.Add(new ElementStyle("Service") { Shape = Shape.Component, Background = "#FEF535", Icon = "" });
            styles.Add(new ElementStyle("Repository") { Shape = Shape.Component, Background = "#FFC100", Icon = "" });



            //Component connection: Session 
            mobileApplication.Uses(sessionFragment, "", "Kotlin");
            mobileApplication.Uses(registerFragment, "", "Kotlin");
            mobileApplication.Uses(verificationFragment, "", "Kotlin");
            sessionFragment.Uses(sessionViewModelFragment, "Usa");
            registerFragment.Uses(registerViewModelFragment, "Usa");
            verificationFragment.Uses(verificationViewModelFragment, "Usa");
            sessionViewModelFragment.Uses(sessionDomainLayer, "Usa");
            registerViewModelFragment.Uses(sessionDomainLayer, "Usa");
            verificationViewModelFragment.Uses(sessionDomainLayer, "Usa");
            sessionViewModelFragment.Uses(sessionRepository, "Lee desde y escribe hasta");
            registerViewModelFragment.Uses(sessionUserRepository, "Lee desde y escribe hasta");
            sessionRepository.Uses(dbSqlite, "Lee desde y escribe hasta");
            sessionUserRepository.Uses(firebasedb, "Lee desde y escribe hasta");
            verificationViewModelFragment.Uses(sms, "Usa");


            //Component connection: Profile 
            mobileApplication.Uses(profileFragment, "", "Kotlin");
            mobileApplication.Uses(settingsFragment, "", "Kotlin");
            profileFragment.Uses(profileViewModelFragment, "Usa");
            settingsFragment.Uses(settingsViewModelFragment, "Usa");
            profileViewModelFragment.Uses(profileDomainLayer, "Usa");
            profileViewModelFragment.Uses(profileRepository, "Lee desde y escribe hasta");
            profileViewModelFragment.Uses(membershipRepository, "Lee desde y escribe hasta");
            settingsViewModelFragment.Uses(profileDomainLayer, "Usa");
            settingsViewModelFragment.Uses(settingsRepository, "Lee desde y escribe hasta");
            settingsViewModelFragment.Uses(membershipRepository, "Lee desde y escribe hasta");
            profileRepository.Uses(firebasedb, "Lee desde y escribe hasta");
            membershipRepository.Uses(firebasedb, "Lee desde y escribe hasta");
            settingsRepository.Uses(firebasedb, "Lee desde y escribe hasta");


            //Component connection: Subscription 
            mobileApplication.Uses(membershipPurchasFragment, "", "Kotlin");
            mobileApplication.Uses(boxPurchaseFragment, "", "Kotlin");
            membershipPurchasFragment.Uses(membershipPurchasViewModelFragment, "Usa");
            boxPurchaseFragment.Uses(boxPurchaseViewModelFragment, "Usa");
            membershipPurchasViewModelFragment.Uses(subscriptionDomainLayer, "Usa");
            membershipPurchasViewModelFragment.Uses(membershipPurchaseRepository, "Lee desde y escribe hasta");
            boxPurchaseViewModelFragment.Uses(subscriptionDomainLayer, "Usa");
            boxPurchaseViewModelFragment.Uses(boxPurchaseRepository, "Lee desde y escribe hasta");

            membershipPurchasViewModelFragment.Uses(paypal, "Usa");
            boxPurchaseViewModelFragment.Uses(paypal, "Usa");
            membershipPurchaseRepository.Uses(firebasedb, "Lee desde y escribe hasta");
            boxPurchaseRepository.Uses(firebasedb, "Lee desde y escribe hasta");


            //Component connection: Inventory 
            mobileApplication.Uses(boxFragment, "", "Kotlin");
            mobileApplication.Uses(sectionFragment, "", "Kotlin");
            mobileApplication.Uses(itemFragment, "", "Kotlin");
            boxFragment.Uses(boxViewModelFragment, "Usa");
            sectionFragment.Uses(sectionsViewModelFragment, "Usa");
            itemFragment.Uses(itemViewModelFragment, "Usa");
            boxViewModelFragment.Uses(inventoryDomainLayer, "Usa");
            boxViewModelFragment.Uses(homeBoxRepository, "Lee desde y escribe hasta");
            boxViewModelFragment.Uses(sharedBoxRepository, "Lee desde y escribe hasta");
            sectionsViewModelFragment.Uses(inventoryDomainLayer, "Usa");
            sectionsViewModelFragment.Uses(homeSectionRepository, "Lee desde y escribe hasta");
            itemViewModelFragment.Uses(inventoryDomainLayer, "Usa");
            itemViewModelFragment.Uses(cloudVisionViewModel, "Usa");
            itemViewModelFragment.Uses(homeItemRepository, "Lee desde y escribe hasta");
            cloudVisionViewModel.Uses(cloudVisionApi, "Usa");
            homeBoxRepository.Uses(firebasedb, "Lee desde y escribe hasta");
            sharedBoxRepository.Uses(firebasedb, "Lee desde y escribe hasta");
            homeSectionRepository.Uses(firebasedb, "Lee desde y escribe hasta");
            homeItemRepository.Uses(firebasedb, "Lee desde y escribe hasta");



            // View - Components Diagram - Session Bounded Context
            ComponentView sessionComponentView = viewSet.CreateComponentView(sessionContext, "Session Bounded Context's Components", "Component Diagram");
            sessionComponentView.PaperSize = PaperSize.A3_Landscape;
            sessionComponentView.Add(mobileApplication);
            sessionComponentView.Add(dbSqlite);
            sessionComponentView.Add(firebasedb);
            sessionComponentView.Add(sms);
            sessionComponentView.AddAllComponents();

            // View - Components Diagram - Profile Bounded Context
            ComponentView profileComponentView = viewSet.CreateComponentView(profileContext, "Profile Bounded Context's Components", "Component Diagram");
            profileComponentView.PaperSize = PaperSize.A4_Landscape;
            profileComponentView.Add(mobileApplication);
            profileComponentView.Add(firebasedb);
            profileComponentView.AddAllComponents();

            // View - Components Diagram - Subscription Bounded Context
            ComponentView subscriptionComponentView = viewSet.CreateComponentView(subscriptionContext, "Subscription Bounded Context's Components", "Component Diagram");
            subscriptionComponentView.PaperSize = PaperSize.A4_Landscape;
            subscriptionComponentView.Add(mobileApplication);
            subscriptionComponentView.Add(paypal);
            subscriptionComponentView.Add(firebasedb);
            subscriptionComponentView.AddAllComponents();

            // View - Components Diagram - Profile Bounded Context
            ComponentView inventoryComponentView = viewSet.CreateComponentView(inventoryContext, "Inventory Bounded Context's Components", "Component Diagram");
            inventoryComponentView.PaperSize = PaperSize.A3_Landscape;
            inventoryComponentView.Add(mobileApplication);
            inventoryComponentView.Add(firebasedb);
            inventoryComponentView.Add(cloudVisionApi);
            inventoryComponentView.AddAllComponents();


            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            DeploymentNode liveWebServer = model.AddDeploymentNode("IlanguageWebApp", "A web server residing in the web server farm, accessed via F5 BIG-IP LTMs.", "Firebase", 1, DictionaryUtils.Create("Location=London"));
            

            DeploymentNode landingPageNode = model
                .AddDeploymentNode("IlanguageLP", "The primary database server.", "Ubuntu 16.04 LTS", 1, DictionaryUtils.Create("Location=London"))
                .AddDeploymentNode("Navegador Web", "The primary, live database server.", "Chrome");
            landingPageNode.Add(landingPage);

            DeploymentNode mobileNode = model
                .AddDeploymentNode("IlanguageMobileApp", "The primary database server.", "Ubuntu 16.04 LTS", 1, DictionaryUtils.Create("Location=London"))
                .AddDeploymentNode("Dispositivo móvil del usuario", "The primary, live database server.", "Android");
            mobileNode.Add(mobileApplication);

            DeploymentNode newNode = model
                .AddDeploymentNode("Google-Cloud Diagram", "The primary, live database server.", "Google Cloud");
            newNode.AddDeploymentNode("API Gateway", "The primary, live database server.", "Docker").Add(apiGateway);

            //newNode.AddDeploymentNode("Payment Context", "The primary, live database server.", "Docker").Add(paymentContext);
            //newNode.AddDeploymentNode("Subscription Context", "The primary, live database server.", "Docker").Add(subscriptionContext);
            //newNode.AddDeploymentNode("Session Context", "The primary, live database server.", "Docker").Add(sessionContext);
            //newNode.AddDeploymentNode("External Tool Context", "The primary, live database server.", "Docker").Add(externalToolsContext);

            DeploymentNode dataBaseNode = model
                .AddDeploymentNode("Azure-Cloud Diagram", "The primary, live database server.", "Azure Database for MySQL");
  

            

            DeploymentNode deployedLandingPage = model
                .AddDeploymentNode("Oracle - Primary", "The primary, live database server.", "Oracle 12c");

            Relationship dataReplicationRelationship = landingPageNode.Uses(liveWebServer, "Call Action To", "");

            DeploymentView liveDeploymentView = viewSet.CreateDeploymentView(tutoringSystem, "Deployment Diagram", "ILanguage Deployment Diagram");
            liveDeploymentView.Add(liveWebServer);
            liveDeploymentView.Add(newNode);
            liveDeploymentView.Add(mobileNode);
            liveDeploymentView.Add(dataBaseNode);
            liveDeploymentView.Add(landingPageNode);
            liveDeploymentView.PaperSize = PaperSize.A3_Landscape;

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}