using Structurizr;
using Structurizr.Api;
using Structurizr.Core.Util;
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
            Component sessionQueryController = sessionContext.AddComponent("Session Query Controller", "REST API endpoints de Session Details", "Spring Boot REST Controller");
            Component sessionsCommandController = sessionContext.AddComponent("Sessions Command Controller", "REST API endpoints de Session", "Spring Boot REST Controller");
            
            Component sessionComandService = sessionContext.AddComponent("Session Command Service", "Provee métodos para Session, pertenece a la capa Application de DDD", "Spring Component");
            Component sessionViewProjection = sessionContext.AddComponent("Session View Projection", "", "Spring Component");
            Component sessionHistoryViewProjection = sessionContext.AddComponent("Session History View Projection", "", "Spring Component");

            Component sessionViewRepository = sessionContext.AddComponent("Session View Repository", "Provee los métodos para la persistencia de datos de Session", "Spring Component");
            Component sessionHistoryViewRepository = sessionContext.AddComponent("Session History View Repository", "Provee los métodos para la persistencia de datos de Session", "Spring Component");

            Component domainLayer = sessionContext.AddComponent("Domain Layer", "Contiene las entidades Core del microservicio", "Spring Component");


            // Components Diagram - User Bounded Context
           
            

            // Components Diagram - Subscription Bounded Context
            Component subscriptionCommandController = subscriptionContext.AddComponent("Subscription Command Controller", "REST API endpoints de Payment", "Spring Boot REST Controller");
            Component subscriptionQueryController = subscriptionContext.AddComponent("Subscriptions Controller", "REST API endpoints de Subscription", "Spring Boot REST Controller");

            Component subscriptionApplicationService = subscriptionContext.AddComponent("Subscription Application Service", "Provee métodos para Subscription, pertenece a la capa Application de DDD", "Spring Component");
            Component subscriptionViewProjection = subscriptionContext.AddComponent("Subscription View Projection", "", "Spring Component");
            Component subscriptionHistoryViewProjection = subscriptionContext.AddComponent("Subscription History View Projection", "", "Spring Component");

            Component subscriptionViewRepository = subscriptionContext.AddComponent("Subscription View Repository", "Provee los métodos para la persistencia de datos de Subscription", "Spring Component");
            Component subscriptionHistoryViewRepository = subscriptionContext.AddComponent("Subscription History View Repository", "Provee los métodos para la persistencia de datos de Subscription", "Spring Component");

            // Components Diagram - External Tools Bounded Context
            //Component externalToolController = externalToolsContext.AddComponent("External Tool Command Controller", "REST API ", "Spring Boot REST Controller");
            //Component externalQueryController = externalToolsContext.AddComponent("External Tool Query Controller", "REST API ", "Spring Boot REST Controller");

            //Component externalApplicationService = externalToolsContext.AddComponent("External Tool Application Service", "", "Spring Component");
            //Component externalToolViewProjection = externalToolsContext.AddComponent("External Tool View Projection", "", "Spring Component");
            //Component externalToolHistoryViewProjection = externalToolsContext.AddComponent("External Tool History View Projection", "", "Spring Component");

            //Component externalToolRespository = externalToolsContext.AddComponent("External Tool Repository", "", "Spring Component");
            //Component externalToolViewRespository = externalToolsContext.AddComponent("External Tool View Repository", "", "Spring Component");
            //Component externalToolHistoryViewRespository = externalToolsContext.AddComponent("External Tool History View Repository", "", "Spring Component");

            //// Components Diagram - Payment Bounded Context
            //Component paymentCommandController = paymentContext.AddComponent("Payment Command Controller", "REST API ", "Spring Boot REST Controller");
            

            //Component transactionCommandController = paymentContext.AddComponent("Transaction Command Controller", "REST API ", "Spring Boot REST Controller");
           

            //Component paymentApplicationService = paymentContext.AddComponent("Payment Application Service", "", "Spring Component");
            //Component transactionApplicationService = paymentContext.AddComponent("Transaction Application Service", "", "Spring Component");

            //Component paypalFacade = paymentContext.AddComponent("Paypal Facade", "", "Spring Component");

          

            // Tags
            

            sessionQueryController.AddTags("Controller");

            sessionsCommandController.AddTags("Controller");
            sessionComandService.AddTags("Service");
            sessionViewProjection.AddTags("Service");
            sessionHistoryViewProjection.AddTags("Service");
            sessionViewRepository.AddTags("Repository");
            sessionHistoryViewRepository.AddTags("Repository");
            domainLayer.AddTags("Repository");

            subscriptionCommandController.AddTags("Controller");
            subscriptionQueryController.AddTags("Controller");
            subscriptionApplicationService.AddTags("Service");
            subscriptionViewProjection.AddTags("Service");
            subscriptionHistoryViewProjection.AddTags("Service");
            subscriptionViewRepository.AddTags("Repository");
            subscriptionHistoryViewRepository.AddTags("Repository");

            
            //externalToolController.AddTags("Controller");
            //externalQueryController.AddTags("Controller");
            //externalApplicationService.AddTags("Service");
            //externalToolViewProjection.AddTags("Service");
            //externalToolHistoryViewProjection.AddTags("Service");
            ////externalToolRespository.AddTags("Repository");
            //externalToolViewRespository.AddTags("Repository");
            //externalToolHistoryViewRespository.AddTags("Repository");
                        
            
            //paymentCommandController.AddTags("Controller");
           
            //transactionCommandController.AddTags("Controller");
            //paymentApplicationService.AddTags("Service");
            //transactionApplicationService.AddTags("Service");
            //paypalFacade.AddTags("Service");


            styles.Add(new ElementStyle("Controller") { Shape = Shape.Component, Background = "#FDFF8B", Icon = "" });
            styles.Add(new ElementStyle("Service") { Shape = Shape.Component, Background = "#FEF535", Icon = "" });
            styles.Add(new ElementStyle("Repository") { Shape = Shape.Component, Background = "#FFC100", Icon = "" });



            //Component connection: Language
            

            //Component connection: Role 
            
            


            //Component connection: Session 
            apiGateway.Uses(sessionQueryController, "", "Java/Spring Boot");
            sessionQueryController.Uses(sessionViewRepository, "Usa");
            sessionQueryController.Uses(sessionHistoryViewRepository, "Usa");
            sessionViewProjection.Uses(sessionViewRepository, "Lee desde y escribe hasta");
            sessionHistoryViewProjection.Uses(sessionHistoryViewRepository, "Lee desde y escribe hasta");
 
            apiGateway.Uses(sessionsCommandController, "", "Java/Spring Boot");
            sessionsCommandController.Uses(sessionComandService, "Llama a los métodos del Service");
            sessionComandService.Uses(domainLayer, "Usa");
            /**/

            //Component connection: Subscription 
            apiGateway.Uses(subscriptionQueryController, "", "Java/Spring Boot");
            apiGateway.Uses(subscriptionCommandController, "", "Java/Spring Boot");
            subscriptionCommandController.Uses(subscriptionApplicationService, "Usa");
            subscriptionApplicationService.Uses(domainLayer, "Usa");
            subscriptionQueryController.Uses(subscriptionViewRepository, "Usa");
            subscriptionQueryController.Uses(subscriptionHistoryViewRepository, "Usa");
            subscriptionViewProjection.Uses(subscriptionViewRepository, "Usa");
            subscriptionHistoryViewProjection.Uses(subscriptionHistoryViewRepository, "Usa");


            //Component connection: Topic Of Interest 
            

            //Component connection: User 
           


            //Component connection: External
            //Google Calendar
            //apiGateway.Uses(externalQueryController, "", "Java/Spring Boot");
            ////externalQueryController.Uses(externalToolRespository, "Lee desde y escribe hasta");
            //externalQueryController.Uses(externalToolViewRespository, "Lee desde y escribe hasta");
            //externalQueryController.Uses(externalToolHistoryViewRespository, "Lee desde y escribe hasta");
            //externalToolViewProjection.Uses(externalToolViewRespository, "Lee desde y escribe hasta");
            //externalToolHistoryViewProjection.Uses(externalToolHistoryViewRespository, "Lee desde y escribe hasta");
            ////externalToolRespository.Uses(externalToolContextDatabase, "Lee desde y escribe hasta");


            ////sms
            //apiGateway.Uses(externalToolController, "", "Java/Spring Boot");
            //externalToolController.Uses(externalApplicationService, "Llama a los métodos del Service");
            //externalApplicationService.Uses(domainLayer, "Usa");
            //externalApplicationService.Uses(sms, "Usa");
           

            ////Component connection: Payment
            //apiGateway.Uses(paymentCommandController, "", "Java/Spring Boot");
            //apiGateway.Uses(transactionCommandController, "", "Java/Spring Boot");
            //transactionCommandController.Uses(transactionApplicationService, "", "Llama a los métodos del Service");
      
            //paymentCommandController.Uses(paymentApplicationService, "", "Llama a los métodos del Service");
            //paymentApplicationService.Uses(paypalFacade, "", "Usa");
            //transactionApplicationService.Uses(domainLayer, "", "Usa");
            //paymentApplicationService.Uses(domainLayer, "", "Usa");
            //paypalFacade.Uses(paypal, "", "Usa");



            // View - Components Diagram - Session Bounded Context
            ComponentView sessionComponentView = viewSet.CreateComponentView(sessionContext, "Session Bounded Context's Components", "Component Diagram");
            sessionComponentView.PaperSize = PaperSize.A4_Landscape;
            sessionComponentView.Add(mobileApplication);
            sessionComponentView.Add(apiGateway);

            sessionComponentView.AddAllComponents();

            // View - Components Diagram - User Bounded Context
            

            // View - Components Diagram - Subscription Bounded Context
            ComponentView subscriptionComponentView = viewSet.CreateComponentView(subscriptionContext, "Subscription Bounded Context's Components", "Component Diagram");
            subscriptionComponentView.PaperSize = PaperSize.A4_Landscape;
            subscriptionComponentView.Add(mobileApplication);
            subscriptionComponentView.Add(apiGateway);
            subscriptionComponentView.Add(domainLayer);
            subscriptionComponentView.AddAllComponents();

            //// View - Components Diagram - External Bounded Context
            //ComponentView externalToolsComponentView = viewSet.CreateComponentView(externalToolsContext, "External Tools Bounded Context's Components", "Component Diagram");
            //externalToolsComponentView.PaperSize = PaperSize.A4_Landscape;
            //externalToolsComponentView.Add(mobileApplication);
            //externalToolsComponentView.Add(apiGateway);
           
            //externalToolsComponentView.Add(sms);
            //externalToolsComponentView.Add(domainLayer);
            //externalToolsComponentView.AddAllComponents();

            //// View - Components Diagram - External Payment Context
            //ComponentView paymentComponentView = viewSet.CreateComponentView(paymentContext, "Payment Bounded Context's Components", "Component Diagram");
            //paymentComponentView.Add(mobileApplication);
            //paymentComponentView.PaperSize = PaperSize.A3_Landscape;
            //paymentComponentView.Add(apiGateway);
            //paymentComponentView.Add(paymentCommandController);
            //paymentComponentView.Add(paypalFacade);
            //paymentComponentView.Add(paypal);
            //paymentComponentView.Add(domainLayer);
            //paymentComponentView.AddAllComponents();


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